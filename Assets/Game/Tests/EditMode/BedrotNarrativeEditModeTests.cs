using System;
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Narrative.Infrastructure;
using Bedrot.Save;
using Bedrot.Shared;
using NUnit.Framework;

namespace Bedrot.Tests.EditMode
{
    public sealed class BedrotNarrativeEditModeTests
    {
        private static readonly CharacterId Mina = new("Mina");
        private static readonly CharacterId Daniel = new("Daniel");
        private static readonly CharacterId Sara = new("Sara");

        [Test]
        public void RelationshipScoreChangesSupportPositiveAndNegativeValues()
        {
            var session = new GameSession(); session.Relationships.ChangeScore(Mina, 3); session.Relationships.ChangeScore(Mina, -5);
            Assert.That(session.Relationships.GetScore(Mina), Is.EqualTo(-2));
        }

        [TestCase("S1_POST_WITHOUT_ACCUSATION", 1, -1, 2, "clip_published_without_accusation")]
        [TestCase("S1_WAIT_FOR_RECORDING", -1, 2, -1, "publication_delayed")]
        [TestCase("S1_PUBLISH_MINA_ACCOUNT", 2, -2, 1, "minas_account_published")]
        [TestCase("S1_NOT_ENOUGH_EVIDENCE", -1, 1, -2, "refused_to_confirm")]
        public void AllFourOpeningChoicesApplyEveryEffect(string choiceId, int mina, int daniel, int sara, string flag)
        {
            Harness harness = CreateHarness(); harness.Start(); harness.Choice(choiceId);
            Assert.That(harness.Session.Relationships.GetScore(Mina), Is.EqualTo(mina));
            Assert.That(harness.Session.Relationships.GetScore(Daniel), Is.EqualTo(daniel));
            Assert.That(harness.Session.Relationships.GetScore(Sara), Is.EqualTo(sara));
            Assert.That(harness.Session.StoryFlags.Contains(new StoryFlagId(flag)), Is.True);
            Assert.That(harness.Session.ChoiceHistory.Contains(new ChoiceId(choiceId)), Is.True);
            Assert.That(harness.Session.NarrativeProgress.IsCompleted(DemoScenarioFactory.OpeningSceneId), Is.True);
        }

        [Test]
        public void MinimumAndCompositeSpecificationsEvaluateFromGameSession()
        {
            var session = new GameSession(); session.Relationships.ChangeScore(Daniel, 2); session.StoryFlags.Set(new StoryFlagId("ready"));
            var minimum = new MinimumRelationshipScoreSpecification(Daniel, 2);
            var composite = new AllSceneConditionsCompositeSpecification(new INarrativeSceneSpecification[] { minimum, new StoryFlagSetSpecification(new StoryFlagId("ready")) });
            Assert.That(minimum.IsSatisfiedBy(session), Is.True); Assert.That(composite.IsSatisfiedBy(session), Is.True);
            Assert.That(new AnySceneConditionsCompositeSpecification(new INarrativeSceneSpecification[] { new MinimumRelationshipScoreSpecification(Mina, 1), minimum }).IsSatisfiedBy(session), Is.True);
        }

        [Test]
        public void HighestPriorityEligibleSceneAndFallbackAreSelected()
        {
            IReadOnlyList<NarrativeSceneDefinition> candidates = DemoScenarioFactory.CreateDemoScenarioDefinitions().Where(x => x.Scene.MajorSceneId == new MajorSceneId("S2")).ToArray();
            var strategy = new HighestPriorityNarrativeSceneSelectionStrategy(); var session = new GameSession();
            session.Relationships.SetScore(Daniel, 2); session.Relationships.SetScore(Mina, 2); session.Relationships.SetScore(Sara, 2);
            Assert.That(strategy.SelectScene(candidates, session).Scene.Id.Value, Is.EqualTo("S2_DANIEL_RECORDING"));
            Assert.That(strategy.SelectScene(candidates, new GameSession()).Scene.Id.Value, Is.EqualTo("S2_DEFAULT_VIRAL"));
        }

        [Test]
        public void EqualPriorityUsesNarrativeSceneIdAsDeterministicTieBreaker()
        {
            NarrativeScene Scene(string id) => new(new NarrativeSceneId(id), new MajorSceneId("S9"), Array.Empty<NarrativeBeat>(), Array.Empty<NarrativeChoice>(), Array.Empty<NarrativeSceneTransition>());
            var candidates = new[] { new NarrativeSceneDefinition(Scene("B"), 10, new AlwaysSatisfiedSpecification()), new NarrativeSceneDefinition(Scene("A"), 10, new AlwaysSatisfiedSpecification()) };
            Assert.That(new HighestPriorityNarrativeSceneSelectionStrategy().SelectScene(candidates, new GameSession()).Scene.Id.Value, Is.EqualTo("A"));
        }

        [TestCase("S1_POST_WITHOUT_ACCUSATION", "S2_SARA_ARTICLE")]
        [TestCase("S1_WAIT_FOR_RECORDING", "S2_DANIEL_RECORDING")]
        [TestCase("S1_PUBLISH_MINA_ACCOUNT", "S2_MINA_CONTEXT")]
        [TestCase("S1_NOT_ENOUGH_EVIDENCE", "S2_DEFAULT_VIRAL")]
        public void SelectingEachOpeningChoicePresentsTheCorrectSceneTwoSubscene(string choiceId, string expectedSceneId)
        {
            Harness harness = CreateHarness(); harness.Start();
            Assert.That(harness.Presentation.Scene.Id, Is.EqualTo(DemoScenarioFactory.OpeningSceneId));
            harness.Choice(choiceId);
            Assert.That(harness.Presentation.Scene.Id.Value, Is.EqualTo(expectedSceneId));
        }

        [Test]
        public void NewGameStartsAtOpeningSceneWithZeroRelationshipScores()
        {
            Harness harness = CreateHarness(); harness.Start();
            Assert.That(harness.Session.NarrativeProgress.CurrentNarrativeSceneId, Is.EqualTo(DemoScenarioFactory.OpeningSceneId));
            Assert.That(new[] { Mina, Daniel, Sara }.Select(harness.Session.Relationships.GetScore), Is.All.Zero);
        }

        [Test]
        public void GameSessionMementoRoundTripRestoresAllAuthoritativeState()
        {
            Harness harness = CreateHarness(); harness.Start(); harness.Choice("S1_WAIT_FOR_RECORDING");
            var factory = new GameSessionMementoFactory(); GameSession restored = factory.Restore(factory.CreateMemento(harness.Session));
            Assert.That(restored.NarrativeProgress.CurrentNarrativeSceneId, Is.EqualTo(new NarrativeSceneId("S2_DANIEL_RECORDING")));
            Assert.That(restored.Relationships.GetScore(Daniel), Is.EqualTo(2));
            Assert.That(restored.ChoiceHistory.Contains(new ChoiceId("S1_WAIT_FOR_RECORDING")), Is.True);
            Assert.That(restored.StoryFlags.Contains(new StoryFlagId("publication_delayed")), Is.True);
            Assert.That(restored.NarrativeProgress.IsCompleted(DemoScenarioFactory.OpeningSceneId), Is.True);
        }

        private static Harness CreateHarness() => new();

        private sealed class Harness
        {
            private readonly GameSessionStore _store = new(); private readonly CapturePresentation _presentation = new();
            private readonly StartNewGameCommandHandler _start; private readonly SelectChoiceCommandHandler _choice;
            public GameSession Session => _store.Current; public CapturePresentation Presentation => _presentation;
            public Harness()
            {
                var repository = new ScriptableObjectNarrativeSceneRepository(DemoScenarioFactory.CreateDemoScenarioDefinitions(), DemoScenarioFactory.OpeningSceneId);
                var events = new GameEventPublisher(); var saves = new MemorySaveRepository(); var save = new SaveGameCommandHandler(_store, new GameSessionMementoFactory(), saves);
                var complete = new CompleteNarrativeSceneCommandHandler(_store, events);
                var next = new SelectNextNarrativeSceneCommandHandler(_store, repository, new HighestPriorityNarrativeSceneSelectionStrategy(), _presentation, events);
                _choice = new SelectChoiceCommandHandler(_store, repository, complete, next, save, events);
                _start = new StartNewGameCommandHandler(_store, repository, new ImmediateSceneLoadingAdapter(), _presentation, events);
            }
            public void Start() => _start.Handle(new StartNewGameCommand());
            public void Choice(string id) => _choice.Handle(new SelectChoiceCommand(new ChoiceId(id)));
        }
        private sealed class ImmediateSceneLoadingAdapter : IUnitySceneLoadingAdapter { public void LoadSceneAsync(string sceneName, Action onLoaded) => onLoaded(); }
        public sealed class CapturePresentation : INarrativePresentationGateway { public NarrativeScene Scene { get; private set; } public void PresentScene(NarrativeScene scene) => Scene = scene; public void EndGame(NarrativeSceneId finalSceneId) { } }
        private sealed class MemorySaveRepository : ISaveGameRepository
        {
            private GameSessionMemento _value; public bool HasSave => _value != null; public void Save(GameSessionMemento memento) => _value = memento;
            public GameSessionMemento Load() => _value; public void Delete() => _value = null;
        }
    }
}
