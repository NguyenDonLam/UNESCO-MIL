using System;
using UnityEngine;

namespace Game.Presentation.MainMenu
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BlinkingPromptPresenter : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float minimumAlpha = 0.15f;
        [SerializeField, Min(0.1f)] private float fadeDuration = 0.8f;

        private SpriteRenderer _spriteRenderer;
        private Color _baseColor;
        private float _elapsed;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
                throw new InvalidOperationException("BlinkingPromptPresenter requires a SpriteRenderer.");

            _baseColor = _spriteRenderer.color;
        }

        private void OnEnable()
        {
            _elapsed = 0f;
        }

        private void Update()
        {
            _elapsed += Time.unscaledDeltaTime;
            float wave = (Mathf.Cos(_elapsed * Mathf.PI / fadeDuration) + 1f) * 0.5f;
            float easedWave = wave * wave * (3f - 2f * wave);
            Color color = _baseColor;
            color.a = _baseColor.a * Mathf.Lerp(minimumAlpha, 1f, easedWave);
            _spriteRenderer.color = color;
        }

        private void OnDisable()
        {
            if (_spriteRenderer != null)
                _spriteRenderer.color = _baseColor;
        }
    }
}
