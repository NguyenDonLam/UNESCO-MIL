using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class UnityBackgroundAdapter : MonoBehaviour, IBackgroundAdapter
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private BackgroundCatalogueAsset catalogue;

        public void ShowBackground(string backgroundCue)
        {
            if (targetImage == null || catalogue == null)
                throw new InvalidOperationException("UnityBackgroundAdapter requires an Image and catalogue.");
            if (!catalogue.TryGetSprite(backgroundCue, out Sprite sprite))
                throw new KeyNotFoundException($"Background cue '{backgroundCue}' is not in the catalogue.");
            targetImage.sprite = sprite;
            targetImage.enabled = true;
        }

        public void HideBackground()
        {
            if (targetImage != null) targetImage.enabled = false;
        }
    }
}
