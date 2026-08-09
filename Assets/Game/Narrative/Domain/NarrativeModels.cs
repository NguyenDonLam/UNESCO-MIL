using System;
using System.Collections.Generic;
using Bedrot.Shared;

namespace Bedrot.Narrative.Domain
{
    public sealed record NarrativeBeat(
        CharacterId? SpeakerId,
        string Text,
        string CharacterSpriteCue = null,
        string BackgroundCue = null,
        string AnimationCue = null,
        string AudioCue = null);

    public sealed record NarrativeChoice(
        ChoiceId Id,
        string Text,
        IReadOnlyList<IChoiceEffect> Effects,
        NarrativeSceneId? DirectDestinationSceneId = null);

    public sealed record NarrativeSceneTransition(
        MajorSceneId? DestinationMajorSceneId,
        NarrativeSceneId? DirectDestinationSceneId = null);

    public sealed record NarrativeScene(
        NarrativeSceneId Id,
        MajorSceneId MajorSceneId,
        IReadOnlyList<NarrativeBeat> Beats,
        IReadOnlyList<NarrativeChoice> Choices,
        IReadOnlyList<NarrativeSceneTransition> Transitions);

    public sealed record NarrativeSceneDefinition(
        NarrativeScene Scene,
        int Priority,
        INarrativeSceneSpecification Specification);

    public interface IChoiceEffect
    {
        void Apply(GameSession gameSession);
    }

    public sealed record RelationshipScoreChoiceEffect(CharacterId CharacterId, int Amount) : IChoiceEffect
    {
        public void Apply(GameSession gameSession) => gameSession.Relationships.ChangeScore(CharacterId, Amount);
    }

    public sealed record SetStoryFlagChoiceEffect(StoryFlagId StoryFlagId) : IChoiceEffect
    {
        public void Apply(GameSession gameSession) => gameSession.StoryFlags.Set(StoryFlagId);
    }

    public sealed record RemoveStoryFlagChoiceEffect(StoryFlagId StoryFlagId) : IChoiceEffect
    {
        public void Apply(GameSession gameSession) => gameSession.StoryFlags.Remove(StoryFlagId);
    }

    public sealed record RecordChoiceChoiceEffect(ChoiceId ChoiceId) : IChoiceEffect
    {
        public void Apply(GameSession gameSession) => gameSession.ChoiceHistory.Record(ChoiceId);
    }

    public sealed record CompleteNarrativeSceneChoiceEffect(NarrativeSceneId NarrativeSceneId) : IChoiceEffect
    {
        public void Apply(GameSession gameSession) => gameSession.NarrativeProgress.Complete(NarrativeSceneId);
    }
}
