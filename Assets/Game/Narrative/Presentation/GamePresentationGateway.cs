using System;
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bedrot.Narrative.Presentation
{
    public sealed class GamePresentationGateway : MonoBehaviour, INarrativePresentationGateway
    {
        [SerializeField] private GameEndingPresenter gameEndingPresenter;

        private SIFTToolkitPresenter _siftToolkit;
        private SIFTInformationCardPresenter _siftCard;
        private ImmediateConsequencePresenter _consequencePresenter;
        private EndOfSceneCaseFilePresenter _caseFilePresenter;
        private CommunityContributionPresenter _communityPresenter;

        private NarrativeScenePresenter _presenter;
        private Action<ChoiceId> _selectChoice;
        private Action<NarrativeSceneId> _completeScene;
        private Action _onPlayAgain;
        private Action<SubmitCommunityContributionCommand> _onSubmitContribution;
        private IGameEventPublisher _events;

        private readonly List<VariableChangeSummary> _pendingChoiceChanges = new();
        private MajorSceneId? _currentMajorSceneId;
        private IDisposable _relationshipSubscription;
        private IDisposable _mediaLiteracySubscription;
        private GameSession _lastGameSession;

        public void Bind(Action<ChoiceId> selectChoice, Action<NarrativeSceneId> completeScene,
            Action onPlayAgain, Action<SubmitCommunityContributionCommand> onSubmitContribution, IGameEventPublisher events)
        {
            _selectChoice = selectChoice;
            _completeScene = completeScene;
            _onPlayAgain = onPlayAgain;
            _onSubmitContribution = onSubmitContribution;
            _events = events;

            _relationshipSubscription?.Dispose();
            _mediaLiteracySubscription?.Dispose();
            _relationshipSubscription = events.Subscribe<RelationshipScoreChangedEvent>(e =>
                _pendingChoiceChanges.Add(new VariableChangeSummary(e.CharacterId.Value, e.CurrentScore - e.PreviousScore)));
            _mediaLiteracySubscription = events.Subscribe<MediaLiteracyScoreChangedEvent>(e =>
                _pendingChoiceChanges.Add(new VariableChangeSummary(e.Metric.ToString(), e.CurrentScore - e.PreviousScore)));
        }

        public void ShowSIFTInformationCard(Action onBeginCheck)
        {
            EnsurePersistentPresenters();
            _siftCard.Show(onBeginCheck);
        }

        public void PresentScene(NarrativeScene scene, GameSession gameSession)
        {
            ResolveActiveScenePresenter();
            EnsurePersistentPresenters();
            _siftToolkit.ShowIcon();

            if (_pendingChoiceChanges.Count > 0)
            {
                var changes = new List<VariableChangeSummary>(_pendingChoiceChanges);
                _pendingChoiceChanges.Clear();
                _consequencePresenter.Show(changes, () => AfterImmediateConsequence(scene, gameSession));
                return;
            }
            AfterImmediateConsequence(scene, gameSession);
        }

        public void EndGame(NarrativeSceneId finalSceneId, GameSession gameSession)
        {
            _events?.Publish(new GameEndedEvent(finalSceneId));
            EnsurePersistentPresenters();
            ResolveActiveGameEndingPresenter();
            _siftToolkit.HideIcon();
            _lastGameSession = gameSession;
            gameEndingPresenter.Bind(_onPlayAgain, ShowCommunityContribution);
            gameEndingPresenter.ShowResults(gameSession);
        }

        private void AfterImmediateConsequence(NarrativeScene scene, GameSession gameSession)
        {
            if (_currentMajorSceneId.HasValue && _currentMajorSceneId.Value != scene.MajorSceneId)
            {
                MajorSceneId completedMajorSceneId = _currentMajorSceneId.Value;
                _caseFilePresenter.Show(
                    $"Hồ sơ {completedMajorSceneId.Value}",
                    BuildLessonText(gameSession),
                    BuildCommunityOutcomeText(gameSession),
                    BuildSnapshot(gameSession),
                    () => PresentSceneNow(scene));
                return;
            }
            PresentSceneNow(scene);
        }

        private void PresentSceneNow(NarrativeScene scene)
        {
            _currentMajorSceneId = scene.MajorSceneId;
            _presenter.Bind(_selectChoice, _completeScene, _events);
            _presenter.PresentScene(scene);
        }

        private void ShowCommunityContribution()
        {
            EnsurePersistentPresenters();
            _communityPresenter.Bind(
                command => { _onSubmitContribution?.Invoke(command); ReturnToEndingScreen(); },
                ReturnToEndingScreen);
            _communityPresenter.Show();
        }

        private void ReturnToEndingScreen()
        {
            if (_lastGameSession != null) gameEndingPresenter.ShowResults(_lastGameSession);
        }

        private void EnsurePersistentPresenters()
        {
            if (_siftToolkit == null) _siftToolkit = SIFTToolkitPresenter.Create(transform);
            if (_siftCard == null) _siftCard = SIFTInformationCardPresenter.Create(transform);
            if (_consequencePresenter == null) _consequencePresenter = ImmediateConsequencePresenter.Create(transform);
            if (_caseFilePresenter == null) _caseFilePresenter = EndOfSceneCaseFilePresenter.Create(transform);
            if (_communityPresenter == null) _communityPresenter = CommunityContributionPresenter.Create(transform);

            // Reopening the card from the toolkit is a plain review: nothing to "begin", just close it again.
            _siftToolkit.Bind(() => _siftCard.Show(() => { }));
        }

        private static string BuildLessonText(GameSession gameSession)
        {
            MediaLiteracyMetric[] metrics = (MediaLiteracyMetric[])Enum.GetValues(typeof(MediaLiteracyMetric));
            MediaLiteracyMetric dominant = metrics.OrderByDescending(gameSession.MediaLiteracy.GetScore).First();
            return $"Trong hồ sơ này, các lựa chọn của bạn thể hiện rõ nhất qua tiêu chí {dominant}.";
        }

        private static string BuildCommunityOutcomeText(GameSession gameSession)
        {
            IReadOnlyDictionary<CharacterId, int> scores = gameSession.Relationships.Scores;
            if (scores.Count == 0) return "Chưa có phản ứng nào từ cộng đồng được ghi nhận.";
            KeyValuePair<CharacterId, int> dominant = scores.OrderByDescending(x => x.Value).First();
            return $"Phản ứng rõ rệt nhất đến từ {dominant.Key.Value}.";
        }

        private static List<VariableChangeSummary> BuildSnapshot(GameSession gameSession)
        {
            var snapshot = new List<VariableChangeSummary>();
            foreach (MediaLiteracyMetric metric in (MediaLiteracyMetric[])Enum.GetValues(typeof(MediaLiteracyMetric)))
                snapshot.Add(new VariableChangeSummary(metric.ToString(), gameSession.MediaLiteracy.GetScore(metric)));
            foreach (KeyValuePair<CharacterId, int> relationship in gameSession.Relationships.Scores.OrderBy(x => x.Key.Value, StringComparer.Ordinal))
                snapshot.Add(new VariableChangeSummary(relationship.Key.Value, relationship.Value));
            return snapshot;
        }

        private void ResolveActiveGameEndingPresenter()
        {
            if (gameEndingPresenter != null && gameEndingPresenter.gameObject.scene == SceneManager.GetActiveScene()) return;
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                GameEndingPresenter candidate = root.GetComponentInChildren<GameEndingPresenter>(true);
                if (candidate != null)
                {
                    gameEndingPresenter = candidate;
                    return;
                }
            }

            gameEndingPresenter = GameEndingPresenter.Create(transform);
        }

        private void ResolveActiveScenePresenter()
        {
            if (_presenter != null && _presenter.gameObject.scene == SceneManager.GetActiveScene()) return;
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                NarrativeScenePresenter candidate = root.GetComponentInChildren<NarrativeScenePresenter>(true);
                if (candidate != null) { _presenter = candidate; return; }
            }
            throw new InvalidOperationException("The active Game scene needs a NarrativeScenePresenter in its hierarchy.");
        }
    }
}
