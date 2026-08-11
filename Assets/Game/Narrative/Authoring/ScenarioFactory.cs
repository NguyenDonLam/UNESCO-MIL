using System;
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    public sealed class ScenarioFactory
    {
        public static NarrativeSceneId BuiltInOpeningSceneId => SceneZeroScenarioFactory.OpeningSceneId;
        public IReadOnlyList<NarrativeSceneDefinition> CreateBuiltInSceneZeroDefinitions() =>
            SceneZeroScenarioFactory.CreateSceneDefinitions();

        public NarrativeScene CreateScenario(NarrativeSceneAsset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (string.IsNullOrWhiteSpace(asset.SceneId)) throw new InvalidOperationException($"Narrative scene asset '{asset.name}' has no scene ID.");
            if (string.IsNullOrWhiteSpace(asset.MajorSceneId)) throw new InvalidOperationException($"Narrative scene '{asset.SceneId}' has no major scene ID.");

            NarrativeBeat[] beats = asset.Beats.Select(x => new NarrativeBeat(
                OptionalCharacterId(x.SpeakerId), x.Text ?? string.Empty, EmptyToNull(x.CharacterSpriteCue),
                EmptyToNull(x.BackgroundCue), EmptyToNull(x.AnimationCue), EmptyToNull(x.AudioCue), x.SpriteSlot)).ToArray();
            NarrativeChoice[] choices = asset.Choices.Select(x => CreateChoice(asset.SceneId, x)).ToArray();
            NarrativeSceneTransition[] transitions = asset.Transitions.Select(x => new NarrativeSceneTransition(
                OptionalMajorSceneId(x.DestinationMajorSceneId), OptionalSceneId(x.DirectDestinationSceneId))).ToArray();
            return new NarrativeScene(new NarrativeSceneId(asset.SceneId), new MajorSceneId(asset.MajorSceneId), beats, choices, transitions);
        }

        public NarrativeSceneDefinition CreateScenarioDefinition(NarrativeSceneCandidateAsset candidate)
        {
            if (candidate == null || candidate.Scene == null || candidate.Specification == null)
                throw new InvalidOperationException("A narrative candidate requires a scene and specification.");
            return new NarrativeSceneDefinition(CreateScenario(candidate.Scene), candidate.Priority, candidate.Specification.CreateSpecification());
        }

        private static NarrativeChoice CreateChoice(string sceneId, NarrativeChoiceData data)
        {
            if (data.RelationshipEffects.Count == 0)
                throw new InvalidOperationException($"Choice '{data.ChoiceId}' in '{sceneId}' requires a relationship effect.");
            var effects = new List<IChoiceEffect>();
            effects.AddRange(data.RelationshipEffects.Select(x => new RelationshipScoreChoiceEffect(new CharacterId(x.CharacterId), x.Amount)));
            effects.AddRange(data.StoryFlagEffects.Select(x => x.Remove
                ? (IChoiceEffect)new RemoveStoryFlagChoiceEffect(new StoryFlagId(x.StoryFlagId))
                : new SetStoryFlagChoiceEffect(new StoryFlagId(x.StoryFlagId))));
            effects.AddRange(data.MediaLiteracyEffects.Select(x => new MediaLiteracyScoreChoiceEffect(x.Metric, x.Amount)));
            ChoiceId id = new(data.ChoiceId);
            return new NarrativeChoice(id, data.Text ?? string.Empty, effects, OptionalSceneId(data.DirectDestinationSceneId));
        }

        private static string EmptyToNull(string value) => string.IsNullOrWhiteSpace(value) ? null : value;
        private static CharacterId? OptionalCharacterId(string value) => string.IsNullOrWhiteSpace(value) ? (CharacterId?)null : new CharacterId(value);
        private static NarrativeSceneId? OptionalSceneId(string value) => string.IsNullOrWhiteSpace(value) ? (NarrativeSceneId?)null : new NarrativeSceneId(value);
        private static MajorSceneId? OptionalMajorSceneId(string value) => string.IsNullOrWhiteSpace(value) ? (MajorSceneId?)null : new MajorSceneId(value);
    }
}
