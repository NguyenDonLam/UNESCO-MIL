using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class UnityBackgroundAdapter : MonoBehaviour, IBackgroundAdapter
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private BackgroundCatalogueAsset catalogue;

        public void ShowBackground(string backgroundCue)
        {
            if (catalogue == null || (targetImage == null && targetSpriteRenderer == null))
                throw new InvalidOperationException("UnityBackgroundAdapter requires a UI Image or SpriteRenderer target and a catalogue.");
            if (!catalogue.TryGetSprite(backgroundCue, out Sprite sprite))
                throw new KeyNotFoundException($"Background cue '{backgroundCue}' is not in the catalogue.");

            if (targetImage != null)
            {
                targetImage.sprite = sprite;
                targetImage.enabled = true;
            }
            else
            {
                targetSpriteRenderer.sprite = sprite;
                targetSpriteRenderer.enabled = true;
            }
        }

        public void HideBackground()
        {
            if (targetImage != null) targetImage.enabled = false;
            if (targetSpriteRenderer != null) targetSpriteRenderer.enabled = false;
        }
    }
}
