using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    [CreateAssetMenu(menuName = "Bedrot/Presentation/Character Sprite Catalogue")]
    public sealed class CharacterSpriteCatalogueAsset : ScriptableObject
    {
        [SerializeField] private List<CharacterAppearanceCueEntry> entries = new();

        public bool TryGetAppearance(string cue, out CharacterAppearanceCueEntry appearance)
        {
            appearance = entries.Find(x => string.Equals(x.Cue, cue, StringComparison.Ordinal));
            return appearance?.HasAppearance == true;
        }

        public bool TryGetSprite(string cue, out Sprite sprite)
        {
            bool found = TryGetAppearance(cue, out CharacterAppearanceCueEntry appearance);
            sprite = appearance?.Sprite;
            return found && sprite != null;
        }

        public bool ContainsCue(string cue) => entries.Exists(x => x.Cue == cue && x.HasAppearance);
    }
}
