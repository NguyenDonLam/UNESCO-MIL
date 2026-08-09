using System;
using System.Collections.Generic;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Narrative.Infrastructure;
using Bedrot.Narrative.Presentation;
using Bedrot.Save;
using Bedrot.Shared;
using UnityEngine;

namespace Bedrot.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class GameCompositionRoot : MonoBehaviour
    {
        [Header("Unity adapters")]
        [SerializeField] private UnitySceneLoadingAdapter sceneLoadingAdapter;
        [SerializeField] private GamePresentationGateway presentationGateway;
        [Header("Authoring (ignored when demo content is enabled)")]
        [SerializeField] private bool useBuiltInDemoContent = true;
        [SerializeField] private NarrativeSceneAsset openingScene;
        [SerializeField] private List<NarrativeSceneCandidateAsset> narrativeCandidates = new();

        private StartNewGameCommandHandler _startHandler;
        private SelectChoiceCommandHandler _choiceHandler;
        private SaveGameCommandHandler _saveHandler;
        private LoadGameCommandHandler _loadHandler;
        private RestartGameCommandHandler _restartHandler;
        public GameSession CurrentGameSession => _sessionStore?.Current;
        private GameSessionStore _sessionStore;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (sceneLoadingAdapter == null) sceneLoadingAdapter = GetComponent<UnitySceneLoadingAdapter>();
            if (presentationGateway == null) presentationGateway = GetComponent<GamePresentationGateway>();
            if (sceneLoadingAdapter == null || presentationGateway == null)
                throw new InvalidOperationException("GameCompositionRoot requires UnitySceneLoadingAdapter and GamePresentationGateway components.");

            var events = new GameEventPublisher();
            _sessionStore = new GameSessionStore();
            INarrativeSceneRepository scenes = CreateNarrativeSceneRepository();
            var selection = new HighestPriorityNarrativeSceneSelectionStrategy();
            var mementos = new GameSessionMementoFactory();
            ISaveGameRepository saves = new JsonSaveGameRepository();
            _saveHandler = new SaveGameCommandHandler(_sessionStore, mementos, saves);
            var complete = new CompleteNarrativeSceneCommandHandler(_sessionStore, events);
            var next = new SelectNextNarrativeSceneCommandHandler(_sessionStore, scenes, selection, presentationGateway, events);
            _choiceHandler = new SelectChoiceCommandHandler(_sessionStore, scenes, complete, next, _saveHandler, events);
            _startHandler = new StartNewGameCommandHandler(_sessionStore, scenes, sceneLoadingAdapter, presentationGateway, events);
            _loadHandler = new LoadGameCommandHandler(_sessionStore, mementos, saves, scenes, presentationGateway);
            _restartHandler = new RestartGameCommandHandler(saves, _startHandler);
            presentationGateway.Bind(DispatchSelectChoiceCommand, events);
        }

        public void DispatchStartNewGameCommand() => _startHandler.Handle(new StartNewGameCommand());
        public void DispatchSelectChoiceCommand(ChoiceId id) => _choiceHandler.Handle(new SelectChoiceCommand(id));
        public void DispatchSaveGameCommand() => _saveHandler.Handle(new SaveGameCommand());
        public void DispatchLoadGameCommand() => _loadHandler.Handle(new LoadGameCommand());
        public void DispatchRestartGameCommand() => _restartHandler.Handle(new RestartGameCommand());

        private INarrativeSceneRepository CreateNarrativeSceneRepository()
        {
            if (useBuiltInDemoContent)
                return new ScriptableObjectNarrativeSceneRepository(DemoScenarioFactory.CreateDemoScenarioDefinitions(), DemoScenarioFactory.OpeningSceneId);
            if (openingScene == null) throw new InvalidOperationException("Assign an opening NarrativeSceneAsset or enable built-in demo content.");
            return new ScriptableObjectNarrativeSceneRepository(narrativeCandidates, openingScene, new ScenarioFactory());
        }
    }
}
