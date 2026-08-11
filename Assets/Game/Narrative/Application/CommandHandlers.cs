using System;
using System.Linq;
using Bedrot.Narrative.Domain;
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
            foreach (string character in new[] { "Linh", "Minh", "Vy" }) session.Relationships.SetScore(new CharacterId(character), 0);
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
        private readonly GameSessionStore _store; private readonly INarrativeSceneRepository _repository;
        private readonly SelectNextNarrativeSceneCommandHandler _nextHandler; private readonly INarrativePresentationGateway _presentation;
        private readonly IGameEventPublisher _events;
        public CompleteNarrativeSceneCommandHandler(GameSessionStore store, INarrativeSceneRepository repository,
            SelectNextNarrativeSceneCommandHandler nextHandler, INarrativePresentationGateway presentation, IGameEventPublisher events)
        { _store = store; _repository = repository; _nextHandler = nextHandler; _presentation = presentation; _events = events; }

        public NarrativeScene Handle(CompleteNarrativeSceneCommand command)
        {
            GameSession session = _store.Current ?? throw new InvalidOperationException("A game has not been started.");
            NarrativeScene current = _repository.GetById(command.SceneId);
            if (!session.NarrativeProgress.IsCompleted(command.SceneId))
            {
                session.NarrativeProgress.Complete(command.SceneId);
                _events.Publish(new NarrativeSceneCompletedEvent(command.SceneId));
            }

            if (command.DirectDestinationSceneId.HasValue)
                return _nextHandler.Handle(new SelectNextNarrativeSceneCommand(default, command.DirectDestinationSceneId));

            NarrativeSceneTransition transition = current.Transitions.FirstOrDefault();
            if (transition?.DirectDestinationSceneId is { } directDestination)
                return _nextHandler.Handle(new SelectNextNarrativeSceneCommand(default, directDestination));
            if (transition?.DestinationMajorSceneId is { } destinationMajorSceneId &&
                _repository.GetCandidatesForMajorScene(destinationMajorSceneId).Count > 0)
                return _nextHandler.Handle(new SelectNextNarrativeSceneCommand(destinationMajorSceneId));

            _presentation.EndGame(current.Id);
            return current;
        }
    }

    public sealed class SelectChoiceCommandHandler
    {
        private readonly GameSessionStore _store; private readonly INarrativeSceneRepository _repository;
        private readonly CompleteNarrativeSceneCommandHandler _completeHandler;
        private readonly IGameEventPublisher _events;
        public SelectChoiceCommandHandler(GameSessionStore store, INarrativeSceneRepository repository,
            CompleteNarrativeSceneCommandHandler completeHandler, IGameEventPublisher events)
        { _store = store; _repository = repository; _completeHandler = completeHandler; _events = events; }

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
                else if (effect is MediaLiteracyScoreChoiceEffect mediaLiteracy)
                {
                    int previous = session.MediaLiteracy.GetScore(mediaLiteracy.Metric);
                    effect.Apply(session);
                    _events.Publish(new MediaLiteracyScoreChangedEvent(mediaLiteracy.Metric, previous, session.MediaLiteracy.GetScore(mediaLiteracy.Metric)));
                }
                else effect.Apply(session);
            }
            session.ChoiceHistory.Record(choice.Id);
            return _completeHandler.Handle(new CompleteNarrativeSceneCommand(current.Id, choice.DirectDestinationSceneId));
        }
    }

    public sealed class RestartGameCommandHandler
    {
        private readonly StartNewGameCommandHandler _start;
        public RestartGameCommandHandler(StartNewGameCommandHandler start) => _start = start;
        public void Handle(RestartGameCommand command) => _start.Handle(new StartNewGameCommand());
    }
}
