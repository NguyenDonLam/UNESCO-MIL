using System;
using System.Collections.Generic;
using Bedrot.Shared;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    [Serializable]
    public sealed class DialogueBleepCueEntry
    {
        public string SpeakerId;
        public AudioClip Clip;
    }

    [CreateAssetMenu(menuName = "Bedrot/Presentation/Dialogue Bleep Catalogue")]
    public sealed class DialogueBleepCatalogueAsset : ScriptableObject
    {
        [SerializeField] private AudioClip narratorClip;
        [SerializeField] private List<DialogueBleepCueEntry> characterEntries = new();

        public AudioClip GetClip(CharacterId? speakerId)
        {
            if (!speakerId.HasValue) return narratorClip;
            DialogueBleepCueEntry entry = characterEntries.Find(candidate =>
                string.Equals(candidate.SpeakerId, speakerId.Value.Value, StringComparison.Ordinal));
            return entry?.Clip;
        }
    }
}
