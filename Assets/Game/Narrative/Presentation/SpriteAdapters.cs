using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public interface ICharacterSpriteAdapter { void ShowSprite(string spriteCue); void HideSprite(); }
    public interface IBackgroundAdapter { void ShowBackground(string backgroundCue); void HideBackground(); }

    public sealed class UnityCharacterSpriteAdapter : MonoBehaviour, ICharacterSpriteAdapter
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private CharacterSpriteCatalogueAsset catalogue;
        public void ShowSprite(string spriteCue)
        {
            if (targetImage == null || catalogue == null) throw new InvalidOperationException("UnityCharacterSpriteAdapter requires an Image and catalogue.");
            if (!catalogue.TryGetSprite(spriteCue, out Sprite sprite)) throw new KeyNotFoundException($"Character sprite cue '{spriteCue}' is not in the catalogue.");
            targetImage.sprite = sprite; targetImage.enabled = true;
        }
        public void HideSprite() { if (targetImage != null) targetImage.enabled = false; }
    }

    public sealed class UnityBackgroundAdapter : MonoBehaviour, IBackgroundAdapter
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private BackgroundCatalogueAsset catalogue;
        public void ShowBackground(string backgroundCue)
        {
            if (targetImage == null || catalogue == null) throw new InvalidOperationException("UnityBackgroundAdapter requires an Image and catalogue.");
            if (!catalogue.TryGetSprite(backgroundCue, out Sprite sprite)) throw new KeyNotFoundException($"Background cue '{backgroundCue}' is not in the catalogue.");
            targetImage.sprite = sprite; targetImage.enabled = true;
        }
        public void HideBackground() { if (targetImage != null) targetImage.enabled = false; }
    }
}
