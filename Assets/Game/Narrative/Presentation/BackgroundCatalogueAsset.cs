using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    [CreateAssetMenu(menuName = "Bedrot/Presentation/Background Catalogue")]
    public sealed class BackgroundCatalogueAsset : ScriptableObject
    {
        [SerializeField] private List<SpriteCueEntry> entries = new();
        public bool TryGetSprite(string cue, out Sprite sprite)
        {
            SpriteCueEntry entry = entries.Find(x => string.Equals(x.Cue, cue, StringComparison.Ordinal));
            sprite = entry?.Sprite;
            return sprite != null;
        }
        public bool ContainsCue(string cue) => entries.Exists(x => x.Cue == cue && x.Sprite != null);
    }
}
