using System.Collections.Generic;
using Bedrot.Relationships.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Domain
{
    public sealed class NarrativeProgressState
    {
        private readonly HashSet<NarrativeSceneId> _completed = new();
        public NarrativeSceneId? CurrentNarrativeSceneId { get; private set; }
        public IReadOnlyCollection<NarrativeSceneId> CompletedNarrativeSceneIds => _completed;
        public void Enter(NarrativeSceneId id) => CurrentNarrativeSceneId = id;
        public void Complete(NarrativeSceneId id) => _completed.Add(id);
        public bool IsCompleted(NarrativeSceneId id) => _completed.Contains(id);
        public void Reset() { CurrentNarrativeSceneId = null; _completed.Clear(); }
    }

    public sealed class ChoiceHistoryState
    {
        private readonly HashSet<ChoiceId> _selected = new();
        public IReadOnlyCollection<ChoiceId> SelectedChoiceIds => _selected;
        public void Record(ChoiceId id) => _selected.Add(id);
        public bool Contains(ChoiceId id) => _selected.Contains(id);
        public void Clear() => _selected.Clear();
    }

    public sealed class StoryFlagState
    {
        private readonly HashSet<StoryFlagId> _flags = new();
        public IReadOnlyCollection<StoryFlagId> StoryFlags => _flags;
        public void Set(StoryFlagId id) => _flags.Add(id);
        public void Remove(StoryFlagId id) => _flags.Remove(id);
        public bool Contains(StoryFlagId id) => _flags.Contains(id);
        public void Clear() => _flags.Clear();
    }

    public sealed class GameSession
    {
        public NarrativeProgressState NarrativeProgress { get; } = new();
        public RelationshipState Relationships { get; } = new();
        public ChoiceHistoryState ChoiceHistory { get; } = new();
        public StoryFlagState StoryFlags { get; } = new();
        public MediaLiteracyState MediaLiteracy { get; } = new();
    }
}
