using System;
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
        private NarrativeScenePresenter _presenter;
        private Action<ChoiceId> _selectChoice;
        private Action<NarrativeSceneId> _completeScene;
        private IGameEventPublisher _events;

        public void Bind(Action<ChoiceId> selectChoice, Action<NarrativeSceneId> completeScene, IGameEventPublisher events)
        { _selectChoice = selectChoice; _completeScene = completeScene; _events = events; }
        public void PresentScene(NarrativeScene scene)
        {
            ResolveActiveScenePresenter();
            _presenter.Bind(_selectChoice, _completeScene, _events); _presenter.PresentScene(scene);
        }
        public void EndGame(NarrativeSceneId finalSceneId)
        {
            _events?.Publish(new GameEndedEvent(finalSceneId));
            if (gameEndingPresenter == null)
                gameEndingPresenter = GameEndingPresenter.Create(transform);
            gameEndingPresenter.ShowYouDied();
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
