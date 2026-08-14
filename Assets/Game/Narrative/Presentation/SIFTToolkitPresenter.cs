using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Persistent collapsible toolkit icon shown in the corner during gameplay. Click to expand/collapse the SIFT steps.</summary>
    public sealed class SIFTToolkitPresenter : MonoBehaviour
    {
        private static readonly Color BorderCyan = new(0.31f, 0.85f, 0.91f);
        private static readonly Color PanelFill = new(0.08f, 0.11f, 0.19f);
        private static readonly Color DescriptionColor = new(0.85f, 0.85f, 0.87f);

        private const float IconSize = 92f;
        private const float RowHeight = 88f;
        private const float RowSpacing = 12f;

        private GameObject _panel;
        private bool _isExpanded;

        public static SIFTToolkitPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("SIFTToolkitView", parent, short.MaxValue - 20);
            root.AddComponent<CanvasGroup>();
            Font pixelFont = RuntimeUIFactory.PixelFont;

            Image iconButton = RuntimeUIFactory.CreateIconSquare(root.transform, "ToolkitIcon", "SIFT", PanelFill, BorderCyan, IconSize, pixelFont);
            RectTransform iconRect = iconButton.rectTransform;
            iconRect.SetTopLeft(24f, 24f, new Vector2(IconSize, IconSize));
            Button iconButtonComponent = iconButton.gameObject.AddComponent<Button>();
            iconButtonComponent.targetGraphic = iconButton;

            SIFTStep[] steps = SIFTStepData.Steps;
            float panelHeight = steps.Length * (RowHeight + RowSpacing) + 40f;
            (RectTransform panelFrame, RectTransform panelContent) = RuntimeUIFactory.CreateBracketPanel(
                root.transform, "StepsPanel", new Vector2(620f, panelHeight), BorderCyan, PanelFill);
            panelFrame.anchorMin = panelFrame.anchorMax = new Vector2(0f, 1f);
            panelFrame.pivot = new Vector2(0f, 1f);
            panelFrame.anchoredPosition = new Vector2(24f, -(IconSize + 48f));

            for (int index = 0; index < steps.Length; index++)
                BuildStepRow(panelContent, steps[index], index, pixelFont);

            SIFTToolkitPresenter presenter = root.AddComponent<SIFTToolkitPresenter>();
            presenter._panel = panelFrame.gameObject;
            iconButtonComponent.onClick.AddListener(presenter.TogglePanel);
            presenter.CollapsePanel();
            presenter.HideIcon();
            return presenter;
        }

        private static void BuildStepRow(Transform panelContent, SIFTStep step, int index, Font pixelFont)
        {
            const float iconSize = 56f;
            var row = new GameObject($"Step{step.Letter}", typeof(RectTransform));
            row.transform.SetParent(panelContent, false);
            RectTransform rowRect = (RectTransform)row.transform;
            rowRect.SetInsets(20f + index * (RowHeight + RowSpacing), RowHeight, 20f, 20f);

            Image icon = RuntimeUIFactory.CreateIconSquare(row.transform, "Icon", step.Letter, step.IconFillColor, step.IconBorderColor, iconSize, pixelFont);
            icon.rectTransform.SetTopLeft(0f, 0f, new Vector2(iconSize, iconSize));

            Text title = RuntimeUIFactory.CreateText(row.transform, "Title", step.Title, 20, TextAnchor.UpperLeft, step.AccentColor, pixelFont);
            title.rectTransform.SetInsets(0f, 28f, iconSize + 16f, 0f);

            Text description = RuntimeUIFactory.CreateText(row.transform, "Description", step.Description, 16, TextAnchor.UpperLeft, DescriptionColor);
            description.rectTransform.SetInsets(30f, 56f, iconSize + 16f, 0f);
        }

        public void ShowIcon() => gameObject.SetActive(true);
        public void HideIcon() { CollapsePanel(); gameObject.SetActive(false); }

        private void TogglePanel()
        {
            _isExpanded = !_isExpanded;
            _panel.SetActive(_isExpanded);
        }

        private void CollapsePanel()
        {
            _isExpanded = false;
            if (_panel != null) _panel.SetActive(false);
        }
    }
}
