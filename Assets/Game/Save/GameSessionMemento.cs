using System;
using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Save
{
    [Serializable]
    public sealed class RelationshipScoreMemento
    {
        public string CharacterId;
        public int Score;
    }

    [Serializable]
    public sealed class GameSessionMemento
    {
        public int Version = 1;
        public string CurrentNarrativeSceneId;
        public List<RelationshipScoreMemento> RelationshipScores = new();
        public List<string> SelectedChoiceIds = new();
        public List<string> StoryFlags = new();
        public List<string> CompletedNarrativeSceneIds = new();
    }

    public sealed class GameSessionMementoFactory
    {
        public GameSessionMemento CreateMemento(GameSession session)
        {
            var result = new GameSessionMemento
            {
                CurrentNarrativeSceneId = session.NarrativeProgress.CurrentNarrativeSceneId?.Value
            };
            foreach (var pair in session.Relationships.Scores)
                result.RelationshipScores.Add(new RelationshipScoreMemento { CharacterId = pair.Key.Value, Score = pair.Value });
            foreach (ChoiceId id in session.ChoiceHistory.SelectedChoiceIds) result.SelectedChoiceIds.Add(id.Value);
            foreach (StoryFlagId id in session.StoryFlags.StoryFlags) result.StoryFlags.Add(id.Value);
            foreach (NarrativeSceneId id in session.NarrativeProgress.CompletedNarrativeSceneIds) result.CompletedNarrativeSceneIds.Add(id.Value);
            return result;
        }

        public GameSession Restore(GameSessionMemento memento)
        {
            if (memento == null) throw new ArgumentNullException(nameof(memento));
            if (memento.Version != 1) throw new InvalidOperationException($"Unsupported save version {memento.Version}.");
            var session = new GameSession();
            if (!string.IsNullOrWhiteSpace(memento.CurrentNarrativeSceneId)) session.NarrativeProgress.Enter(new NarrativeSceneId(memento.CurrentNarrativeSceneId));
            foreach (RelationshipScoreMemento item in memento.RelationshipScores) session.Relationships.SetScore(new CharacterId(item.CharacterId), item.Score);
            foreach (string id in memento.SelectedChoiceIds) session.ChoiceHistory.Record(new ChoiceId(id));
            foreach (string id in memento.StoryFlags) session.StoryFlags.Set(new StoryFlagId(id));
            foreach (string id in memento.CompletedNarrativeSceneIds) session.NarrativeProgress.Complete(new NarrativeSceneId(id));
            return session;
        }
    }
}
