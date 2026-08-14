using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Shown once at the start of the game, before the opening scene, to introduce the four SIFT steps.</summary>
    public sealed class SIFTInformationCardPresenter : MonoBehaviour
    {
        private static readonly Color BorderCyan = new(0.31f, 0.85f, 0.91f);
        private static readonly Color CardFill = new(0.08f, 0.11f, 0.19f);
        private static readonly Color TitleBarFill = new(0.12f, 0.22f, 0.29f);
        private static readonly Color TitleTextColor = new(0.80f, 0.96f, 0.98f);
        private static readonly Color DescriptionColor = new(0.85f, 0.85f, 0.87f);
        private static readonly Color DividerColor = new(1f, 1f, 1f, 0.18f);

        private const float IconSize = 84f;
        private const float RowHeight = 132f;
        private const float RowSpacing = 20f;

        private CanvasGroup _canvasGroup;
        private Action _onBeginCheck;

        public static SIFTInformationCardPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("SIFTInformationCardView", parent, short.MaxValue - 5);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            RuntimeUIFactory.CreateDimBackground(root.transform, 0.92f);

            Font pixelFont = RuntimeUIFactory.PixelFont;
            (_, RectTransform content) = RuntimeUIFactory.CreateBracketPanel(
                root.transform, "Card", new Vector2(980f, 900f), BorderCyan, CardFill);

            BuildTitleBar(content, pixelFont);
            BuildSteps(content, pixelFont);
            BuildDivider(content);
            Button beginButton = BuildBeginButton(content, pixelFont);

            SIFTInformationCardPresenter presenter = root.AddComponent<SIFTInformationCardPresenter>();
            presenter._canvasGroup = canvasGroup;
            beginButton.onClick.AddListener(presenter.OnBeginCheckClicked);
            presenter.Hide();
            return presenter;
        }

        private static void BuildTitleBar(Transform content, Font pixelFont)
        {
            var titleBar = new GameObject("TitleBar", typeof(RectTransform), typeof(Image));
            titleBar.transform.SetParent(content, false);
            ((RectTransform)titleBar.transform).SetInsets(0f, 96f, 0f, 0f);
            titleBar.GetComponent<Image>().color = TitleBarFill;

            Text title = RuntimeUIFactory.CreateText(titleBar.transform, "Title", "TRƯỚC KHI BẮT ĐẦU", 40,
                TextAnchor.MiddleCenter, TitleTextColor, pixelFont);
            RuntimeUIFactory.Stretch(title.rectTransform);

            var underline = new GameObject("Underline", typeof(RectTransform), typeof(Image));
            underline.transform.SetParent(content, false);
            ((RectTransform)underline.transform).SetInsets(96f, 4f, 0f, 0f);
            underline.GetComponent<Image>().color = BorderCyan;
        }

        private static void BuildSteps(Transform content, Font pixelFont)
        {
            var stepsContainer = new GameObject("Steps", typeof(RectTransform));
            stepsContainer.transform.SetParent(content, false);
            ((RectTransform)stepsContainer.transform).SetInsets(130f, 620f, 50f, 50f);

            SIFTStep[] steps = SIFTStepData.Steps;
            for (int index = 0; index < steps.Length; index++)
                BuildStepRow(stepsContainer.transform, steps[index], index, pixelFont);
        }

        private static void BuildStepRow(Transform stepsContainer, SIFTStep step, int index, Font pixelFont)
        {
            var row = new GameObject($"Step{step.Letter}", typeof(RectTransform));
            row.transform.SetParent(stepsContainer, false);
            RectTransform rowRect = (RectTransform)row.transform;
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.offsetMin = new Vector2(0f, -(index * (RowHeight + RowSpacing) + RowHeight));
            rowRect.offsetMax = new Vector2(0f, -(index * (RowHeight + RowSpacing)));

            Image icon = RuntimeUIFactory.CreateIconSquare(row.transform, "Icon", step.Letter, step.IconFillColor, step.IconBorderColor, IconSize, pixelFont);
            icon.rectTransform.SetTopLeft(0f, 0f, new Vector2(IconSize, IconSize));

            Text title = RuntimeUIFactory.CreateText(row.transform, "Title", step.Title, 26, TextAnchor.UpperLeft, step.AccentColor, pixelFont);
            title.rectTransform.SetInsets(4f, 60f, IconSize + 20f, 0f);

            Text description = RuntimeUIFactory.CreateText(row.transform, "Description", step.Description, 20, TextAnchor.UpperLeft, DescriptionColor);
            description.rectTransform.SetInsets(66f, 60f, IconSize + 20f, 0f);
        }

        private static void BuildDivider(Transform content)
        {
            var divider = new GameObject("Divider", typeof(RectTransform), typeof(Image));
            divider.transform.SetParent(content, false);
            RectTransform dividerRect = (RectTransform)divider.transform;
            dividerRect.anchorMin = new Vector2(0f, 0f);
            dividerRect.anchorMax = new Vector2(1f, 0f);
            dividerRect.pivot = new Vector2(0.5f, 0f);
            dividerRect.anchoredPosition = new Vector2(0f, 108f);
            dividerRect.sizeDelta = new Vector2(-100f, 2f);
            divider.GetComponent<Image>().color = DividerColor;
        }

        private static Button BuildBeginButton(Transform content, Font pixelFont)
        {
            Button beginButton = RuntimeUIFactory.CreateOutlinedButton(content, "BeginCheckButton", "BẮT ĐẦU KIỂM TRA", CardFill, BorderCyan, pixelFont, 30);
            RectTransform buttonRect = beginButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.sizeDelta = new Vector2(360f, 72f);
            buttonRect.anchoredPosition = new Vector2(0f, 36f);
            return beginButton;
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
