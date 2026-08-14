using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Shown once at the start of the game, before the opening scene, to introduce the four SIFT steps.</summary>
    public sealed class SIFTInformationCardPresenter : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Action _onBeginCheck;

        public static SIFTInformationCardPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("SIFTInformationCardView", parent, short.MaxValue - 5);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            RuntimeUIFactory.CreateDimBackground(root.transform, 0.92f);

            var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(root.transform, false);
            RectTransform cardRect = (RectTransform)card.transform;
            cardRect.anchorMin = cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(900f, 620f);
            card.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.98f);

            RuntimeUIFactory.CreateText(card.transform, "Title", "Bộ công cụ SIFT", 42, TextAnchor.MiddleCenter, Color.white)
                .rectTransform.SetInsets(0f, 500f, 40f, 40f);

            string body = "Trước khi tin vào một thông tin, hãy dùng 4 bước SIFT:\n\n" +
                          "S — Dừng lại: Kiểm tra cảm xúc trước khi phản ứng.\n" +
                          "I — Kiểm tra nguồn: Nguồn tin này có đáng tin không?\n" +
                          "F — Tìm nguồn tốt hơn: Có nguồn uy tín nào đưa tin này không?\n" +
                          "T — Truy nguyên gốc: Tìm bối cảnh và nguồn gốc ban đầu.";
            RuntimeUIFactory.CreateText(card.transform, "Body", body, 26, TextAnchor.UpperLeft, new Color(0.9f, 0.9f, 0.9f))
                .rectTransform.SetInsets(120f, 400f, 40f, 40f);

            Button beginButton = RuntimeUIFactory.CreateButton(card.transform, "BeginCheckButton", "Bắt đầu kiểm tra", new Color(0.2f, 0.55f, 0.3f), 30);
            RectTransform buttonRect = beginButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.sizeDelta = new Vector2(320f, 72f);
            buttonRect.anchoredPosition = new Vector2(0f, 40f);

            SIFTInformationCardPresenter presenter = root.AddComponent<SIFTInformationCardPresenter>();
            presenter._canvasGroup = canvasGroup;
            beginButton.onClick.AddListener(presenter.OnBeginCheckClicked);
            presenter.Hide();
            return presenter;
        }

        public void Show(Action onBeginCheck)
        {
            _onBeginCheck = onBeginCheck;
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void OnBeginCheckClicked()
        {
            Hide();
            _onBeginCheck?.Invoke();
        }
    }
}
