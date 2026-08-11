using System;
using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using UnityEngine;

namespace Bedrot.Narrative.Authoring
{
    [Serializable]
    public sealed class NarrativeBeatData
    {
        public string SpeakerId;
        [TextArea(2, 6)] public string Text;
        public string CharacterSpriteCue;
        public CharacterSpriteSlot SpriteSlot = CharacterSpriteSlot.Auto;
        public string BackgroundCue;
        public string AnimationCue;
        public string AudioCue;
    }

    [Serializable]
    public sealed class RelationshipChoiceEffectData
    {
        public string CharacterId;
        public int Amount;
    }

    [Serializable]
    public sealed class StoryFlagChoiceEffectData
    {
        public string StoryFlagId;
        public bool Remove;
    }

    [Serializable]
    public sealed class MediaLiteracyChoiceEffectData
    {
        public Bedrot.Narrative.Domain.MediaLiteracyMetric Metric;
        public int Amount;
    }

    [Serializable]
    public sealed class NarrativeChoiceData
    {
        public string ChoiceId;
        [TextArea(2, 4)] public string Text;
        public List<RelationshipChoiceEffectData> RelationshipEffects = new();
        public List<StoryFlagChoiceEffectData> StoryFlagEffects = new();
        public List<MediaLiteracyChoiceEffectData> MediaLiteracyEffects = new();
        public string DirectDestinationSceneId;
    }

    [Serializable]
    public sealed class NarrativeSceneTransitionData
    {
        public string DestinationMajorSceneId;
        public string DirectDestinationSceneId;
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Scene", fileName = "NarrativeScene")]
    public sealed class NarrativeSceneAsset : ScriptableObject
    {
        public string SceneId;
        public string MajorSceneId;
        public List<NarrativeBeatData> Beats = new();
        public List<NarrativeChoiceData> Choices = new();
        public List<NarrativeSceneTransitionData> Transitions = new();
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Candidate", fileName = "NarrativeSceneCandidate")]
    public sealed class NarrativeSceneCandidateAsset : ScriptableObject
    {
        public NarrativeSceneAsset Scene;
        public int Priority;
        public NarrativeSceneSpecificationAsset Specification;
    }
}
