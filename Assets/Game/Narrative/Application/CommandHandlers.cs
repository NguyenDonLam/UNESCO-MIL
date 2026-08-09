using System;
using System.Linq;
using Bedrot.Narrative.Domain;
using Bedrot.Save;
using Bedrot.Shared;

namespace Bedrot.Narrative.Application
{
    public sealed class StartNewGameCommandHandler
    {
        private readonly GameSessionStore _store; private readonly INarrativeSceneRepository _repository;
        private readonly IUnitySceneLoadingAdapter _loader; private readonly INarrativePresentationGateway _presentation;
        private readonly IGameEventPublisher _events; private readonly string _gameSceneName;
        public StartNewGameCommandHandler(GameSessionStore store, INarrativeSceneRepository repository, IUnitySceneLoadingAdapter loader,
            INarrativePresentationGateway presentation, IGameEventPublisher events, string gameSceneName = "Game")
        { _store = store; _repository = repository; _loader = loader; _presentation = presentation; _events = events; _gameSceneName = gameSceneName; }

        public void Handle(StartNewGameCommand command)
        {
            GameSession session = _store.CreateNew();
            foreach (string character in new[] { "Mina", "Daniel", "Sara" }) session.Relationships.SetScore(new CharacterId(character), 0);
            _loader.LoadSceneAsync(_gameSceneName, () =>
            {
                NarrativeScene opening = _repository.GetOpeningScene();
                session.NarrativeProgress.Enter(opening.Id);
                _events.Publish(new NewGameStartedEvent(opening.Id));
                _events.Publish(new NarrativeSceneEnteredEvent(opening.Id));
                _presentation.PresentScene(opening);
            });
        }
    }

    public sealed class SelectNextNarrativeSceneCommandHandler
    {
        private readonly GameSessionStore _store; private readonly INarrativeSceneRepository _repository;
        private readonly INarrativeSceneSelectionStrategy _selectionStrategy; private readonly INarrativePresentationGateway _presentation;
        private readonly IGameEventPublisher _events;
        public SelectNextNarrativeSceneCommandHandler(GameSessionStore store, INarrativeSceneRepository repository,
            INarrativeSceneSelectionStrategy selectionStrategy, INarrativePresentationGateway presentation, IGameEventPublisher events)
        { _store = store; _repository = repository; _selectionStrategy = selectionStrategy; _presentation = presentation; _events = events; }

        public NarrativeScene Handle(SelectNextNarrativeSceneCommand command)
        {
            NarrativeScene scene = command.DirectDestinationSceneId.HasValue
                ? _repository.GetById(command.DirectDestinationSceneId.Value)
                : _selectionStrategy.SelectScene(_repository.GetCandidatesForMajorScene(command.MajorSceneId), _store.Current).Scene;
            _store.Current.NarrativeProgress.Enter(scene.Id);
            _events.Publish(new NarrativeSceneSelectedEvent(scene.Id));
            _events.Publish(new NarrativeSceneEnteredEvent(scene.Id));
            _presentation.PresentScene(scene);
            return scene;
        }
    }

    public sealed class CompleteNarrativeSceneCommandHandler
    {
        private readonly GameSessionStore _store; private readonly IGameEventPublisher _events;
        public CompleteNarrativeSceneCommandHandler(GameSessionStore store, IGameEventPublisher events) { _store = store; _events = events; }
        public void Handle(CompleteNarrativeSceneCommand command)
        { _store.Current.NarrativeProgress.Complete(command.SceneId); _events.Publish(new NarrativeSceneCompletedEvent(command.SceneId)); }
    }

    public sealed class SelectChoiceCommandHandler
    {
        private readonly GameSessionStore _store; private readonly INarrativeSceneRepository _repository;
        private readonly CompleteNarrativeSceneCommandHandler _completeHandler; private readonly SelectNextNarrativeSceneCommandHandler _nextHandler;
        private readonly SaveGameCommandHandler _saveHandler; private readonly IGameEventPublisher _events;
        public SelectChoiceCommandHandler(GameSessionStore store, INarrativeSceneRepository repository,
            CompleteNarrativeSceneCommandHandler completeHandler, SelectNextNarrativeSceneCommandHandler nextHandler,
            SaveGameCommandHandler saveHandler, IGameEventPublisher events)
        { _store = store; _repository = repository; _completeHandler = completeHandler; _nextHandler = nextHandler; _saveHandler = saveHandler; _events = events; }

        public NarrativeScene Handle(SelectChoiceCommand command)
        {
            GameSession session = _store.Current ?? throw new InvalidOperationException("A game has not been started.");
            NarrativeSceneId currentId = session.NarrativeProgress.CurrentNarrativeSceneId ?? throw new InvalidOperationException("There is no current narrative scene.");
            NarrativeScene current = _repository.GetById(currentId);
            NarrativeChoice choice = current.Choices.SingleOrDefault(x => x.Id == command.ChoiceId)
                ?? throw new InvalidOperationException($"Choice '{command.ChoiceId}' does not belong to scene '{currentId}'.");
            _events.Publish(new ChoiceSelectedEvent(choice.Id));
            foreach (IChoiceEffect effect in choice.Effects)
            {
                if (effect is RelationshipScoreChoiceEffect relationship)
                {
                    int previous = session.Relationships.GetScore(relationship.CharacterId);
                    effect.Apply(session);
                    _events.Publish(new RelationshipScoreChangedEvent(relationship.CharacterId, previous, session.Relationships.GetScore(relationship.CharacterId)));
                }
                else effect.Apply(session);
            }
            session.ChoiceHistory.Record(choice.Id);
            _completeHandler.Handle(new CompleteNarrativeSceneCommand(current.Id));
            NarrativeSceneTransition transition = current.Transitions.FirstOrDefault();
            if (choice.DirectDestinationSceneId.HasValue)
            {
                NarrativeScene selected = _nextHandler.Handle(new SelectNextNarrativeSceneCommand(default, choice.DirectDestinationSceneId));
                _saveHandler.Handle(new SaveGameCommand()); return selected;
            }
            if (transition?.DirectDestinationSceneId != null || transition?.DestinationMajorSceneId != null)
            {
                NarrativeScene selected = _nextHandler.Handle(new SelectNextNarrativeSceneCommand(
                    transition.DestinationMajorSceneId ?? default, transition.DirectDestinationSceneId));
                _saveHandler.Handle(new SaveGameCommand()); return selected;
            }
            _events.Publish(new GameEndedEvent(current.Id));
            _saveHandler.Handle(new SaveGameCommand()); return current;
        }
    }

    public sealed class SaveGameCommandHandler
    {
        private readonly GameSessionStore _store; private readonly GameSessionMementoFactory _factory; private readonly ISaveGameRepository _repository;
        public SaveGameCommandHandler(GameSessionStore store, GameSessionMementoFactory factory, ISaveGameRepository repository)
        { _store = store; _factory = factory; _repository = repository; }
        public void Handle(SaveGameCommand command) { if (_store.Current != null) _repository.Save(_factory.CreateMemento(_store.Current)); }
    }

    public sealed class LoadGameCommandHandler
    {
        private readonly GameSessionStore _store; private readonly GameSessionMementoFactory _factory; private readonly ISaveGameRepository _repository;
        private readonly INarrativeSceneRepository _scenes; private readonly INarrativePresentationGateway _presentation;
        public LoadGameCommandHandler(GameSessionStore store, GameSessionMementoFactory factory, ISaveGameRepository repository,
            INarrativeSceneRepository scenes, INarrativePresentationGateway presentation)
        { _store = store; _factory = factory; _repository = repository; _scenes = scenes; _presentation = presentation; }
        public void Handle(LoadGameCommand command)
        { GameSession restored = _factory.Restore(_repository.Load()); _store.Replace(restored); _presentation.PresentScene(_scenes.GetById(restored.NarrativeProgress.CurrentNarrativeSceneId.Value)); }
    }

    public sealed class RestartGameCommandHandler
    {
        private readonly ISaveGameRepository _save; private readonly StartNewGameCommandHandler _start;
        public RestartGameCommandHandler(ISaveGameRepository save, StartNewGameCommandHandler start) { _save = save; _start = start; }
        public void Handle(RestartGameCommand command) { _save.Delete(); _start.Handle(new StartNewGameCommand()); }
    }
}
