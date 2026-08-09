using System.Collections;
using System;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Narrative.Infrastructure;
using Bedrot.Narrative.Presentation;
using Bedrot.Save;
using Bedrot.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Bedrot.Tests.PlayMode
{
    public sealed class NarrativePresentationPlayModeTests
    {
        [UnityTest]
        public IEnumerator GameSceneLoadsFromBuildSettings()
        {
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Game"));
        }

        [UnityTest]
        public IEnumerator DialogueAdvancesAndChoiceButtonsAppear()
        {
            NarrativeScenePresenter presenter = CreatePresenterRig(out DialoguePresenter dialogue, out ChoicePresenter choices);
            var scene = DemoScenarioFactory.CreateDemoScenarioDefinitions()[0].Scene;
            presenter.Bind(_ => { }, new GameEventPublisher()); presenter.PresentScene(scene);
            Assert.That(presenter.CurrentBeatIndex, Is.Zero);
            for (int i = 1; i < scene.Beats.Count; i++) presenter.AdvanceDialogue();
            presenter.AdvanceDialogue(); yield return null;
            Assert.That(choices.gameObject.activeSelf, Is.True);
            UnityEngine.Object.Destroy(presenter.transform.root.gameObject);
        }

        [UnityTest]
        public IEnumerator OpeningSceneAppearsInNarrativePresenter()
        {
            NarrativeScenePresenter presenter = CreatePresenterRig(out _, out _);
            presenter.Bind(_ => { }, new GameEventPublisher());
            presenter.PresentScene(DemoScenarioFactory.CreateDemoScenarioDefinitions()[0].Scene);
            yield return null;
            Assert.That(presenter.CurrentScene.Id, Is.EqualTo(DemoScenarioFactory.OpeningSceneId));
            UnityEngine.Object.Destroy(presenter.transform.root.gameObject);
        }

        [UnityTest]
        public IEnumerator SelectedChoiceChangesThePresentedScene()
        {
            var store = new GameSessionStore();
            var repository = new ScriptableObjectNarrativeSceneRepository(DemoScenarioFactory.CreateDemoScenarioDefinitions(), DemoScenarioFactory.OpeningSceneId);
            var events = new GameEventPublisher(); var presentation = new CapturePresentation();
            var save = new SaveGameCommandHandler(store, new GameSessionMementoFactory(), new MemorySaveRepository());
            var next = new SelectNextNarrativeSceneCommandHandler(store, repository, new HighestPriorityNarrativeSceneSelectionStrategy(), presentation, events);
            var choice = new SelectChoiceCommandHandler(store, repository, new CompleteNarrativeSceneCommandHandler(store, events), next, save, events);
            new StartNewGameCommandHandler(store, repository, new ImmediateSceneLoadingAdapter(), presentation, events).Handle(new StartNewGameCommand());
            choice.Handle(new SelectChoiceCommand(new ChoiceId("S1_WAIT_FOR_RECORDING")));
            yield return null;
            Assert.That(presentation.Scene.Id.Value, Is.EqualTo("S2_DANIEL_RECORDING"));
        }

        private static NarrativeScenePresenter CreatePresenterRig(out DialoguePresenter dialogue, out ChoicePresenter choices)
        {
            var root = new GameObject("NarrativeTestRig");
            var dialogueObject = new GameObject("Dialogue", typeof(RectTransform)); dialogueObject.transform.SetParent(root.transform);
            dialogue = dialogueObject.AddComponent<DialoguePresenter>();
            var choiceObject = new GameObject("Choices", typeof(RectTransform)); choiceObject.transform.SetParent(root.transform);
            choices = choiceObject.AddComponent<ChoicePresenter>();
            var button = new GameObject("ChoiceButton", typeof(RectTransform), typeof(Image), typeof(Button)); button.transform.SetParent(choiceObject.transform);
            var label = new GameObject("Label", typeof(RectTransform), typeof(Text)); label.transform.SetParent(button.transform);
            var presenter = root.AddComponent<NarrativeScenePresenter>();
            SetField(presenter, "dialoguePresenter", dialogue); SetField(presenter, "choicePresenter", choices);
            return presenter;
        }
        private static void SetField(object target, string name, object value) => target.GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(target, value);

        private sealed class ImmediateSceneLoadingAdapter : IUnitySceneLoadingAdapter { public void LoadSceneAsync(string sceneName, Action onLoaded) => onLoaded(); }
        private sealed class CapturePresentation : INarrativePresentationGateway
        {
            public NarrativeScene Scene { get; private set; }
            public void PresentScene(NarrativeScene scene) => Scene = scene;
            public void EndGame(NarrativeSceneId finalSceneId) { }
        }
        private sealed class MemorySaveRepository : ISaveGameRepository
        {
            private GameSessionMemento _memento;
            public bool HasSave => _memento != null;
            public void Save(GameSessionMemento memento) => _memento = memento;
            public GameSessionMemento Load() => _memento;
            public void Delete() => _memento = null;
        }
    }
}
