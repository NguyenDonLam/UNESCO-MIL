using System;
using System.Collections.Generic;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Narrative.Infrastructure;
using Bedrot.Narrative.Presentation;
using Bedrot.Shared;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bedrot.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class GameCompositionRoot : MonoBehaviour
    {
        [Header("Unity adapters")]
        [SerializeField] private UnitySceneLoadingAdapter sceneLoadingAdapter;
        [SerializeField] private GamePresentationGateway presentationGateway;
        [Header("Authoring")]
        [FormerlySerializedAs("useBuiltInDemoContent")]
        [SerializeField] private bool useBuiltInSceneZeroContent = true;
        [SerializeField] private NarrativeSceneAsset openingScene;
        [SerializeField] private List<NarrativeSceneCandidateAsset> narrativeCandidates = new();

        private StartNewGameCommandHandler _startHandler;
        private SelectChoiceCommandHandler _choiceHandler;
        private CompleteNarrativeSceneCommandHandler _completeHandler;
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
            var next = new SelectNextNarrativeSceneCommandHandler(_sessionStore, scenes, selection, presentationGateway, events);
            _completeHandler = new CompleteNarrativeSceneCommandHandler(_sessionStore, scenes, next, presentationGateway, events);
            _choiceHandler = new SelectChoiceCommandHandler(_sessionStore, scenes, _completeHandler, events);
            _startHandler = new StartNewGameCommandHandler(_sessionStore, scenes, sceneLoadingAdapter, presentationGateway, events);
            _restartHandler = new RestartGameCommandHandler(_startHandler);
            presentationGateway.Bind(DispatchSelectChoiceCommand, DispatchCompleteNarrativeSceneCommand, events);
        }

        public void DispatchStartNewGameCommand() => _startHandler.Handle(new StartNewGameCommand());
        public void DispatchSelectChoiceCommand(ChoiceId id) => _choiceHandler.Handle(new SelectChoiceCommand(id));
        public void DispatchCompleteNarrativeSceneCommand(NarrativeSceneId id) => _completeHandler.Handle(new CompleteNarrativeSceneCommand(id));
        public void DispatchRestartGameCommand() => _restartHandler.Handle(new RestartGameCommand());

        private INarrativeSceneRepository CreateNarrativeSceneRepository()
        {
            if (useBuiltInSceneZeroContent)
            {
                var factory = new ScenarioFactory();
                var definitions = new List<NarrativeSceneDefinition>(factory.CreateBuiltInNarrativeDefinitions());
                foreach (NarrativeSceneCandidateAsset candidate in narrativeCandidates)
                    definitions.Add(factory.CreateScenarioDefinition(candidate));
                return new ScriptableObjectNarrativeSceneRepository(definitions, ScenarioFactory.BuiltInOpeningSceneId);
            }
            if (openingScene == null) throw new InvalidOperationException("Assign an opening NarrativeSceneAsset or enable built-in Scene 0 content.");
            return new ScriptableObjectNarrativeSceneRepository(narrativeCandidates, openingScene, new ScenarioFactory());
        }
    }
}
