using System;
using System.Collections;
using Bedrot.Bootstrap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Presentation.MainMenu
{
    public sealed class MainMenuPresenter : MonoBehaviour
    {
        [Header("Scene layers")]
        [SerializeField] private SpriteRenderer bedroomRenderer;
        [SerializeField] private SpriteRenderer bedRenderer;

        [Header("Legacy menu artwork")]
        [SerializeField] private SpriteRenderer startButtonRenderer;
        [SerializeField] private SpriteRenderer settingButtonRenderer;

        [Header("Transition")]
        [SerializeField, Min(0.1f)] private float fadeDuration = 0.8f;
        [SerializeField] private GameCompositionRoot gameCompositionRoot;

        private Camera _camera;
        private Image _fadeOverlay;
        private bool _isTransitioning;

        private void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
                throw new InvalidOperationException("MainMenuPresenter requires a camera tagged MainCamera.");
            if (gameCompositionRoot == null)
                throw new InvalidOperationException("MainMenuPresenter requires a GameCompositionRoot reference.");

            _camera.backgroundColor = Color.black;
            FitSceneLayers();
            HideLegacyButtons();
            BuildFadeOverlay();
        }

        private void Update()
        {
            if (!_isTransitioning && Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
                StartNewGame();
        }

        public void StartNewGame()
        {
            if (!_isTransitioning) StartCoroutine(FadeToNewGame());
        }

        private void FitSceneLayers()
        {
            ResolveSceneLayerReferences();
            FitSceneLayer(bedroomRenderer, true);
            FitSceneLayer(bedRenderer, false);
        }

        private void ResolveSceneLayerReferences()
        {
            Transform visualLayers = transform.Find("VisualLayers");
            if (visualLayers == null) return;
            if (bedroomRenderer == null)
                bedroomRenderer = visualLayers.Find("Bedroom")?.GetComponent<SpriteRenderer>();
            if (bedRenderer == null)
                bedRenderer = visualLayers.Find("Bed")?.GetComponent<SpriteRenderer>();
        }

        private void FitSceneLayer(SpriteRenderer spriteRenderer, bool cover)
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                Debug.LogError("Main menu scene layer is missing its SpriteRenderer or sprite.", this);
                return;
            }

            float cameraHeight = _camera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * _camera.aspect;
            Sprite sprite = spriteRenderer.sprite;
            float widthScale = cameraWidth / (sprite.rect.width / sprite.pixelsPerUnit);
            float heightScale = cameraHeight / (sprite.rect.height / sprite.pixelsPerUnit);
            spriteRenderer.transform.localScale = Vector3.one * (cover ? Mathf.Max(widthScale, heightScale) : widthScale);
        }

        private void HideLegacyButtons()
        {
            if (startButtonRenderer != null) startButtonRenderer.gameObject.SetActive(false);
            if (settingButtonRenderer != null) settingButtonRenderer.gameObject.SetActive(false);
        }

        private void BuildFadeOverlay()
        {
            var canvasObject = new GameObject("SceneTransitionCanvas", typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var fadeObject = new GameObject("FadeOverlay", typeof(RectTransform), typeof(Image));
            fadeObject.transform.SetParent(canvasObject.transform, false);
            var fadeRect = fadeObject.GetComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.offsetMin = Vector2.zero;
            fadeRect.offsetMax = Vector2.zero;
            _fadeOverlay = fadeObject.GetComponent<Image>();
            _fadeOverlay.color = Color.clear;
            _fadeOverlay.raycastTarget = false;
        }

        private IEnumerator FadeToNewGame()
        {
            _isTransitioning = true;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / fadeDuration);
                float easedProgress = progress * progress * (3f - 2f * progress);
                _fadeOverlay.color = new Color(0f, 0f, 0f, easedProgress);
                yield return null;
            }

            _fadeOverlay.color = Color.black;
            gameCompositionRoot.DispatchStartNewGameCommand();
        }
    }
}
