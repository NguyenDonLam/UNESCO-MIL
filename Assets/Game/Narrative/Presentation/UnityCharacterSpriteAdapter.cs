using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class UnityCharacterSpriteAdapter : MonoBehaviour, ICharacterSpriteAdapter
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private CharacterSpriteCatalogueAsset catalogue;

        public void ShowSprite(string spriteCue) => ShowAppearance(spriteCue);

        public void ShowAppearance(string appearanceCue)
        {
            if (targetImage == null || catalogue == null)
                throw new InvalidOperationException("UnityCharacterSpriteAdapter requires an Image and catalogue.");
            if (!catalogue.TryGetAppearance(appearanceCue, out CharacterAppearanceCueEntry appearance))
                throw new KeyNotFoundException($"Character appearance cue '{appearanceCue}' is not in the catalogue or has no Sprite/Animation Controller.");

            if (appearance.IsAnimated) ShowAnimation(appearance, appearanceCue);
            else ShowStaticSprite(appearance.Sprite);
        }

        private void ShowStaticSprite(Sprite sprite)
        {
            StopAnimation();
            targetImage.sprite = sprite;
            targetImage.enabled = true;
        }

        private void ShowAnimation(CharacterAppearanceCueEntry appearance, string appearanceCue)
        {
            if (targetAnimator == null)
                throw new InvalidOperationException($"Character appearance cue '{appearanceCue}' uses animation, but UnityCharacterSpriteAdapter has no Animator assigned.");

            if (appearance.Sprite != null) targetImage.sprite = appearance.Sprite;
            targetImage.enabled = true;
            targetAnimator.enabled = true;
            targetAnimator.runtimeAnimatorController = appearance.AnimationController;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
            if (!string.IsNullOrWhiteSpace(appearance.AnimationStateName))
            {
                targetAnimator.Play(appearance.AnimationStateName, 0, 0f);
                targetAnimator.Update(0f);
            }
        }

        private void StopAnimation()
        {
            if (targetAnimator == null) return;
            targetAnimator.enabled = false;
            targetAnimator.runtimeAnimatorController = null;
        }

        public void HideSprite()
        {
            StopAnimation();
            if (targetImage != null) targetImage.enabled = false;
        }
    }
}
