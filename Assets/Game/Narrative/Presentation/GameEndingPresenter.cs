using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Presents the terminal screen after the final narrative scene.</summary>
    public sealed class GameEndingPresenter : MonoBehaviour
    {
        private const string EndingMessage = "you died";
        private CanvasGroup _canvasGroup;
        private Text _messageText;

        public static GameEndingPresenter Create(Transform parent)
        {
            var root = new GameObject("GameEndingView", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(GameEndingPresenter));
            if (parent != null) root.transform.SetParent(parent, false);

            RectTransform rootRect = root.GetComponent<RectTransform>();
            Stretch(rootRect);

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var background = new GameObject("BlackBackground", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(root.transform, false);
            Stretch(background.GetComponent<RectTransform>());
            background.GetComponent<Image>().color = Color.black;

            var message = new GameObject("EndingMessage", typeof(RectTransform), typeof(Text));
            message.transform.SetParent(root.transform, false);
            Stretch(message.GetComponent<RectTransform>());
            Text text = message.GetComponent<Text>();
            text.text = EndingMessage;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 64;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 24;
            text.resizeTextMaxSize = 64;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameEndingPresenter presenter = root.GetComponent<GameEndingPresenter>();
            presenter._canvasGroup = root.GetComponent<CanvasGroup>();
            presenter._messageText = text;
            presenter.Hide();
            return presenter;
        }

        public void ShowYouDied()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_messageText != null) _messageText.text = EndingMessage;
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void Hide()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
