using System;
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Narrative.Infrastructure;
using Bedrot.Narrative.Presentation;
using Bedrot.Shared;
using NUnit.Framework;

namespace Bedrot.Tests.EditMode
{
    public sealed class BedrotNarrativeEditModeTests
    {
        private static readonly CharacterId Linh = new("Linh");
        private static readonly CharacterId Minh = new("Minh");
        private static readonly CharacterId Vy = new("Vy");

        [Test]
        public void RelationshipScoreChangesSupportPositiveAndNegativeValues()
        {
            var session = new GameSession();
            session.Relationships.ChangeScore(Linh, 3);
            session.Relationships.ChangeScore(Linh, -5);
            Assert.That(session.Relationships.GetScore(Linh), Is.EqualTo(-2));
        }

        [Test]
        public void SceneZeroChoiceAppliesRelationshipMediaLiteracyAndStoryEffects()
        {
            Harness harness = CreateHarness();
            harness.Start();
            harness.Choice("CHOICE_0_1_B");

            Assert.That(harness.Session.Relationships.GetScore(Linh), Is.EqualTo(1));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Evidence), Is.EqualTo(2));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Transparency), Is.EqualTo(1));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Crisis), Is.EqualTo(-1));
            Assert.That(harness.Session.StoryFlags.Contains(new StoryFlagId("s00_verification_prioritized")), Is.True);
            Assert.That(harness.Session.ChoiceHistory.Contains(new ChoiceId("CHOICE_0_1_B")), Is.True);
            Assert.That(harness.Session.NarrativeProgress.IsCompleted(ScenarioFactory.BuiltInOpeningSceneId), Is.True);
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S00_RESPONSE_01_B"));
        }

        [Test]
        public void DialogueOnlyResponseCompletesIntoSharedMergeNode()
        {
            Harness harness = CreateHarness();
            harness.Start();
            harness.Choice("CHOICE_0_1_C");
            harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S00_MERGE_01"));
            Assert.That(harness.Session.NarrativeProgress.IsCompleted(new NarrativeSceneId("S00_RESPONSE_01_C")), Is.True);
        }

        [Test]
        public void SceneZeroCanReachItsEndingAcrossAllThreeChoices()
        {
            Harness harness = CreateHarness();
            harness.Start();
            harness.Choice("CHOICE_0_1_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_0_2_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_0_3_B"); harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S00_END"));
            Assert.That(harness.Session.Relationships.GetScore(Linh), Is.EqualTo(2));
            Assert.That(harness.Session.Relationships.GetScore(Minh), Is.EqualTo(1));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Evidence), Is.EqualTo(6));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Transparency), Is.EqualTo(4));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Crisis), Is.EqualTo(-4));
        }

        [Test]
        public void SceneZeroEndingConnectsToFutureSceneOneCandidate()
        {
            NarrativeScene sceneOne = new(new NarrativeSceneId("S01_THE_LEAKED_IMAGE"), new MajorSceneId("S01"),
                new[] { new NarrativeBeat(null, "Scene 1") }, Array.Empty<NarrativeChoice>(), Array.Empty<NarrativeSceneTransition>());
            NarrativeSceneDefinition[] definitions = new ScenarioFactory().CreateBuiltInSceneZeroDefinitions()
                .Concat(new[] { new NarrativeSceneDefinition(sceneOne, 0, new AlwaysSatisfiedSpecification()) }).ToArray();
            Harness harness = new(definitions);
            harness.Start();
            harness.GoToEndingWithVerificationChoices();
            harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S01_THE_LEAKED_IMAGE"));
        }

        [Test]
        public void BuiltInSceneOneRunsThroughItsSharedMergeNodesAndAppliesAuthoredEffects()
        {
            Harness harness = new(new ScenarioFactory().CreateBuiltInNarrativeDefinitions());
            harness.Start();
            harness.GoToEndingWithVerificationChoices();
            harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S01_THE_LEAKED_IMAGE"));

            harness.Choice("CHOICE_1_1_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S01_MERGE_01"));
            harness.Choice("CHOICE_1_2_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S01_MERGE_02"));
            harness.Choice("CHOICE_1_3_B"); harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S01_END"));
            Assert.That(harness.Session.Relationships.GetScore(Vy), Is.EqualTo(4));
            Assert.That(harness.Session.MediaLiteracy.GetScore(MediaLiteracyMetric.Privacy), Is.EqualTo(4));
            Assert.That(harness.Session.StoryFlags.Contains(new StoryFlagId("s01_minimum_necessary_evidence_published")), Is.True);
        }

        [Test]
        public void BuiltInSceneTwoRunsThroughItsSharedMergeNodesAndResolvesEveryFinalBranch()
        {
            Harness harness = new(new ScenarioFactory().CreateBuiltInNarrativeDefinitions());
            harness.Start();
            harness.GoToEndingWithVerificationChoices();
            harness.CompleteCurrent();
            harness.Choice("CHOICE_1_1_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_1_2_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_1_3_B"); harness.CompleteCurrent();
            harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S02_THE_SECRET_DOCUMENT"));
            int minhScoreBeforeSceneTwo = harness.Session.Relationships.GetScore(Minh);

            harness.Choice("CHOICE_2_1_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S02_MERGE_01"));
            harness.Choice("CHOICE_2_2_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S02_MERGE_02"));
            harness.Choice("CHOICE_2_3_B"); harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S02_END"));
            Assert.That(harness.Session.Relationships.GetScore(Minh) - minhScoreBeforeSceneTwo, Is.EqualTo(5));
            Assert.That(harness.Session.StoryFlags.Contains(new StoryFlagId("s02_responsible_financial_disclosure")), Is.True);
        }

        [Test]
        public void BuiltInSceneThreeRunsThroughSharedMergeNodesAndSelectsBalancedOutcome()
        {
            Harness harness = StartAtSceneThree();
            int linhBefore = harness.Session.Relationships.GetScore(Linh);
            int vyBefore = harness.Session.Relationships.GetScore(Vy);

            harness.Choice("CHOICE_3_1_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S03_MERGE_01"));
            harness.Choice("CHOICE_3_2_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S03_MERGE_02"));
            harness.Choice("CHOICE_3_3_B"); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S03_UPDATE"));
            harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S03_OUTCOME_BALANCED"));
            Assert.That(harness.Session.Relationships.GetScore(Linh) - linhBefore, Is.EqualTo(1));
            Assert.That(harness.Session.Relationships.GetScore(Vy) - vyBefore, Is.EqualTo(5));
            Assert.That(harness.Session.StoryFlags.Contains(new StoryFlagId("s03_proportionate_update_published")), Is.True);
        }

        [TestCase("CHOICE_3_1_A", "CHOICE_3_2_A", "CHOICE_3_3_B", "S03_OUTCOME_OVER_WARNING")]
        [TestCase("CHOICE_3_1_C", "CHOICE_3_2_C", "CHOICE_3_3_B", "S03_OUTCOME_UNDER_COMMUNICATION")]
        [TestCase("CHOICE_3_1_A", "CHOICE_3_2_B", "CHOICE_3_3_C", "S03_OUTCOME_MIXED")]
        public void SceneThreeConsequenceSelectionIsDeterministic(string first, string second, string third, string expectedOutcome)
        {
            Harness harness = StartAtSceneThree();
            harness.Choice(first); harness.CompleteCurrent();
            harness.Choice(second); harness.CompleteCurrent();
            harness.Choice(third); harness.CompleteCurrent();
            harness.CompleteCurrent();

            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo(expectedOutcome));
        }

        [Test]
        public void MinimumAndCompositeSpecificationsEvaluateFromGameSession()
        {
            var session = new GameSession(); session.Relationships.ChangeScore(Minh, 2); session.StoryFlags.Set(new StoryFlagId("ready"));
            var minimum = new MinimumRelationshipScoreSpecification(Minh, 2);
            var composite = new AllSceneConditionsCompositeSpecification(new INarrativeSceneSpecification[] { minimum, new StoryFlagSetSpecification(new StoryFlagId("ready")) });
            Assert.That(minimum.IsSatisfiedBy(session), Is.True);
            Assert.That(composite.IsSatisfiedBy(session), Is.True);
            Assert.That(new AnySceneConditionsCompositeSpecification(new INarrativeSceneSpecification[] { new MinimumRelationshipScoreSpecification(Linh, 1), minimum }).IsSatisfiedBy(session), Is.True);
        }

        [Test]
        public void HighestPriorityEligibleSceneAndFallbackAreSelected()
        {
            NarrativeScene Scene(string id) => new(new NarrativeSceneId(id), new MajorSceneId("S09"), Array.Empty<NarrativeBeat>(), Array.Empty<NarrativeChoice>(), Array.Empty<NarrativeSceneTransition>());
            var candidates = new[]
            {
                new NarrativeSceneDefinition(Scene("TRUST"), 10, new MinimumRelationshipScoreSpecification(Minh, 2)),
                new NarrativeSceneDefinition(Scene("DEFAULT"), 0, new AlwaysSatisfiedSpecification())
            };
            var strategy = new HighestPriorityNarrativeSceneSelectionStrategy();
            var trusted = new GameSession(); trusted.Relationships.SetScore(Minh, 2);
            Assert.That(strategy.SelectScene(candidates, trusted).Scene.Id.Value, Is.EqualTo("TRUST"));
            Assert.That(strategy.SelectScene(candidates, new GameSession()).Scene.Id.Value, Is.EqualTo("DEFAULT"));
        }

        [Test]
        public void NewGameStartsAtSceneZeroWithZeroRelationshipAndMediaLiteracyScores()
        {
            Harness harness = CreateHarness(); harness.Start();
            Assert.That(harness.Session.NarrativeProgress.CurrentNarrativeSceneId, Is.EqualTo(ScenarioFactory.BuiltInOpeningSceneId));
            Assert.That(new[] { Linh, Minh, Vy, new CharacterId("Cô Hương") }.Select(harness.Session.Relationships.GetScore), Is.All.Zero);
            Assert.That(Enum.GetValues(typeof(MediaLiteracyMetric)).Cast<MediaLiteracyMetric>().Select(harness.Session.MediaLiteracy.GetScore), Is.All.Zero);
        }

        [Test]
        public void EndingResultsShowOnlyTheInsightsForTheHighestScores()
        {
            Harness harness = CreateHarness();
            harness.Start();
            harness.Session.Relationships.ChangeScore(Linh, 2);
            harness.Session.Relationships.ChangeScore(Vy, -1);
            harness.Session.MediaLiteracy.ChangeScore(MediaLiteracyMetric.Evidence, 3);

            string results = GameEndingPresenter.BuildResultsText(harness.Session);

            Assert.That(results, Does.Contain("Người kiểm chứng thông tin"));
            Assert.That(results, Does.Contain("Xây dựng niềm tin với Linh"));
            Assert.That(results, Does.Not.Contain("+3"));
            Assert.That(results, Does.Not.Contain("+2"));
            Assert.That(results, Does.Not.Contain("-1"));
            Assert.That(results, Does.Not.Contain("Evidence):"));
        }

        [Test]
        public void EndingPresenterBuildsSeparateFinalInsightPanelContent()
        {
            Harness harness = CreateHarness();
            harness.Start();
            harness.Session.Relationships.ChangeScore(Linh, 2);
            harness.Session.MediaLiteracy.ChangeScore(MediaLiteracyMetric.Evidence, 3);

            FinalInsightViewModel viewModel = GameEndingPresenter.BuildFinalInsightViewModel(harness.Session);

            Assert.That(viewModel.ScreenTitle, Is.EqualTo("NHẬN XÉT CUỐI GAME"));
            Assert.That(viewModel.MediaLiteracyPanelLabel, Is.EqualTo("NHẬN XÉT MIL"));
            Assert.That(viewModel.MediaLiteracyInsightTitle, Is.EqualTo("Người kiểm chứng thông tin"));
            Assert.That(viewModel.MediaLiteracyInsightBody, Does.Contain("nguồn tin, bối cảnh và bằng chứng"));
            Assert.That(viewModel.RelationshipPanelLabel, Is.EqualTo("NHẬN XÉT QUAN HỆ"));
            Assert.That(viewModel.RelationshipInsightTitle, Is.EqualTo("Xây dựng niềm tin với Linh"));
            Assert.That(viewModel.RelationshipInsightBody, Does.Contain("Linh phản ứng tích cực nhất"));
            Assert.That(viewModel.RelationshipCharacterId, Is.EqualTo("Linh"));
        }

        [Test]
        public void EndingResultsUseBalancedInsightsWhenHighestScoresAreTied()
        {
            Harness harness = CreateHarness();
            harness.Start();

            string results = GameEndingPresenter.BuildResultsText(harness.Session);

            Assert.That(results, Does.Contain("Người ra quyết định cân bằng"));
            Assert.That(results, Does.Contain("Nhận được sự tin tưởng của Cô Hương"));
            Assert.That(results, Does.Contain("Xây dựng niềm tin với Linh"));
            Assert.That(results, Does.Contain("Thuyết phục được Minh"));
            Assert.That(results, Does.Contain("Tạo được sự đồng cảm với Vy"));
        }

        [Test]
        public void BuiltInSceneZeroNarrativePassesValidation()
        {
            IReadOnlyList<NarrativeSceneDefinition> definitions = new ScenarioFactory().CreateBuiltInSceneZeroDefinitions();
            IReadOnlyList<NarrativeValidationResult> results = new NarrativeSceneValidator().Validate(definitions,
                ScenarioFactory.BuiltInOpeningSceneId, new[] { Linh, Minh, new CharacterId("Vy") });
            Assert.That(results.Where(x => x.Severity == NarrativeValidationSeverity.Error), Is.Empty);
        }

        [Test]
        public void BuiltInSceneZeroAndOneNarrativePassesValidation()
        {
            IReadOnlyList<NarrativeSceneDefinition> definitions = new ScenarioFactory().CreateBuiltInNarrativeDefinitions();
            IReadOnlyList<NarrativeValidationResult> results = new NarrativeSceneValidator().Validate(definitions,
                ScenarioFactory.BuiltInOpeningSceneId, new[] { Linh, Minh, Vy, new CharacterId("Cô Hương") });
            Assert.That(results.Where(x => x.Severity == NarrativeValidationSeverity.Error), Is.Empty);
        }

        private static Harness CreateHarness() => new(new ScenarioFactory().CreateBuiltInSceneZeroDefinitions());

        private static Harness StartAtSceneThree()
        {
            Harness harness = new(new ScenarioFactory().CreateBuiltInNarrativeDefinitions());
            harness.Start();
            harness.GoToEndingWithVerificationChoices(); harness.CompleteCurrent();
            harness.Choice("CHOICE_1_1_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_1_2_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_1_3_B"); harness.CompleteCurrent(); harness.CompleteCurrent();
            harness.Choice("CHOICE_2_1_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_2_2_B"); harness.CompleteCurrent();
            harness.Choice("CHOICE_2_3_B"); harness.CompleteCurrent(); harness.CompleteCurrent();
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo("S03_PUBLIC_EMERGENCY"));
            return harness;
        }

        private sealed class Harness
        {
            private readonly GameSessionStore _store = new();
            private readonly CapturePresentation _presentation = new();
            private readonly StartNewGameCommandHandler _start;
            private readonly SelectChoiceCommandHandler _choice;
            private readonly CompleteNarrativeSceneCommandHandler _complete;
            public GameSession Session => _store.Current;
            public CapturePresentation Presentation => _presentation;

            public Harness(IEnumerable<NarrativeSceneDefinition> definitions)
            {
                var repository = new ScriptableObjectNarrativeSceneRepository(definitions, ScenarioFactory.BuiltInOpeningSceneId);
                var events = new GameEventPublisher();
                var next = new SelectNextNarrativeSceneCommandHandler(_store, repository, new HighestPriorityNarrativeSceneSelectionStrategy(), _presentation, events);
                _complete = new CompleteNarrativeSceneCommandHandler(_store, repository, next, _presentation, events);
                _choice = new SelectChoiceCommandHandler(_store, repository, _complete, events);
                _start = new StartNewGameCommandHandler(_store, repository, new ImmediateSceneLoadingAdapter(), _presentation, events);
            }

            public void Start() => _start.Handle(new StartNewGameCommand());
            public void Choice(string id) => _choice.Handle(new SelectChoiceCommand(new ChoiceId(id)));
            public void CompleteCurrent() => _complete.Handle(new CompleteNarrativeSceneCommand(Session.NarrativeProgress.CurrentNarrativeSceneId.Value));
            public void GoToEndingWithVerificationChoices()
            {
                Choice("CHOICE_0_1_B"); CompleteCurrent();
                Choice("CHOICE_0_2_B"); CompleteCurrent();
                Choice("CHOICE_0_3_B"); CompleteCurrent();
            }
        }

        private sealed class ImmediateSceneLoadingAdapter : IUnitySceneLoadingAdapter
        {
            public void LoadSceneAsync(string sceneName, Action onLoaded) => onLoaded();
        }

        public sealed class CapturePresentation : INarrativePresentationGateway
        {
            public NarrativeScene Scene { get; private set; }
            public NarrativeSceneId? EndedAt { get; private set; }
            public void ShowSIFTInformationCard(Action onBeginCheck) => onBeginCheck?.Invoke();
            public void PresentScene(NarrativeScene scene, GameSession gameSession) => Scene = scene;
            public void EndGame(NarrativeSceneId finalSceneId, GameSession gameSession) => EndedAt = finalSceneId;
        }
    }
}
