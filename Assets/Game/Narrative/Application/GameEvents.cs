using System;
using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Application
{
    public interface IGameEvent { }
    public sealed record NewGameStartedEvent(NarrativeSceneId OpeningSceneId) : IGameEvent;
    public sealed record NarrativeSceneEnteredEvent(NarrativeSceneId SceneId) : IGameEvent;
    public sealed record NarrativeBeatChangedEvent(NarrativeSceneId SceneId, int BeatIndex) : IGameEvent;
    public sealed record ChoiceSelectedEvent(ChoiceId ChoiceId) : IGameEvent;
    public sealed record RelationshipScoreChangedEvent(CharacterId CharacterId, int PreviousScore, int CurrentScore) : IGameEvent;
    public sealed record MediaLiteracyScoreChangedEvent(MediaLiteracyMetric Metric, int PreviousScore, int CurrentScore) : IGameEvent;
    public sealed record NarrativeSceneCompletedEvent(NarrativeSceneId SceneId) : IGameEvent;
    public sealed record NarrativeSceneSelectedEvent(NarrativeSceneId SceneId) : IGameEvent;
    public sealed record GameEndedEvent(NarrativeSceneId FinalSceneId) : IGameEvent;

    public interface IGameEventPublisher
    {
        IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;
        void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent;
    }

    public sealed class GameEventPublisher : IGameEventPublisher
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent
        {
            Type type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out List<Delegate> handlers)) _handlers[type] = handlers = new();
            handlers.Add(handler);
            return new EventSubscription(() => handlers.Remove(handler));
        }

        public void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out List<Delegate> handlers)) return;
            foreach (Delegate handler in handlers.ToArray()) ((Action<TEvent>)handler)(gameEvent);
        }

        private sealed class EventSubscription : IDisposable
        {
            private Action _unsubscribe;
            public EventSubscription(Action unsubscribe) => _unsubscribe = unsubscribe;
            public void Dispose() { _unsubscribe?.Invoke(); _unsubscribe = null; }
        }
    }
}
