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
        [SerializeField, Min(0.01f)] private float displayScale = 1.7141f;
        [SerializeField, Min(1f)] private float bopPixels = 8f;
        [SerializeField, Min(1f)] private float bopsPerAnimationLoop = 2f;
        private bool _isAnimated;
        private float _horizontalDirection = 1f;
        private bool _hasBaseAnchoredPosition;
        private Vector2 _baseAnchoredPosition;

        private static readonly int[] PixelBopFrameOffsets = { 0, 0, 0, 4, 8, 8, 4, 0 };

        public void ShowSprite(string spriteCue) => ShowAppearance(spriteCue);

        public void ShowAppearance(string appearanceCue)
        {
            if (targetImage == null || catalogue == null)
                throw new InvalidOperationException("UnityCharacterSpriteAdapter requires an Image and catalogue.");
            if (!catalogue.TryGetAppearance(appearanceCue, out CharacterAppearanceCueEntry appearance))
                throw new KeyNotFoundException($"Character appearance cue '{appearanceCue}' is not in the catalogue or has no Sprite/Animation Controller.");

            CaptureBaseAnchoredPosition();
            _horizontalDirection = appearance.FlipHorizontal ? -1f : 1f;
            if (appearance.IsAnimated) ShowAnimation(appearance, appearanceCue);
            else ShowStaticSprite(appearance.Sprite);
        }

        private void ShowStaticSprite(Sprite sprite)
        {
            StopAnimation();
            targetImage.sprite = sprite;
            targetImage.rectTransform.localScale = new Vector3(
                displayScale * _horizontalDirection, displayScale, displayScale);
            targetImage.rectTransform.anchoredPosition = _baseAnchoredPosition;
            targetImage.enabled = true;
        }

        private void ShowAnimation(CharacterAppearanceCueEntry appearance, string appearanceCue)
        {
            if (targetAnimator == null)
                throw new InvalidOperationException($"Character appearance cue '{appearanceCue}' uses animation, but UnityCharacterSpriteAdapter has no Animator assigned.");

            if (appearance.Sprite != null) targetImage.sprite = appearance.Sprite;
            targetImage.enabled = true;
            targetImage.rectTransform.localScale = Vector3.one;
            targetAnimator.enabled = true;
            targetAnimator.runtimeAnimatorController = appearance.AnimationController;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
            if (!string.IsNullOrWhiteSpace(appearance.AnimationStateName))
            {
                targetAnimator.Play(appearance.AnimationStateName, 0, 0f);
                targetAnimator.Update(0f);
            }
            _isAnimated = true;
            ApplyAnimatedDisplayTransform();
        }

        private void LateUpdate()
        {
            if (_isAnimated && targetAnimator != null && targetAnimator.enabled)
                ApplyAnimatedDisplayTransform();
        }

        private void ApplyAnimatedDisplayTransform()
        {
            targetImage.rectTransform.localScale = new Vector3(
                displayScale * _horizontalDirection,
                displayScale,
                displayScale);

            float normalizedTime = targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float loopPhase = normalizedTime - Mathf.Floor(normalizedTime);
            int frame = Mathf.FloorToInt(loopPhase * PixelBopFrameOffsets.Length * bopsPerAnimationLoop)
                        % PixelBopFrameOffsets.Length;
            float frameOffset = PixelBopFrameOffsets[frame] / 8f * bopPixels;
            targetImage.rectTransform.anchoredPosition =
                _baseAnchoredPosition + new Vector2(0f, Mathf.Round(frameOffset));
        }

        private void CaptureBaseAnchoredPosition()
        {
            if (_hasBaseAnchoredPosition) return;
            _baseAnchoredPosition = targetImage.rectTransform.anchoredPosition;
            _hasBaseAnchoredPosition = true;
        }

        private void StopAnimation()
        {
            _isAnimated = false;
            if (_hasBaseAnchoredPosition && targetImage != null)
                targetImage.rectTransform.anchoredPosition = _baseAnchoredPosition;
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
