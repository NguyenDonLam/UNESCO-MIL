using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Persistent collapsible toolkit icon shown in the corner during gameplay. Click to expand/collapse the SIFT steps.</summary>
    public sealed class SIFTToolkitPresenter : MonoBehaviour
    {
        private static readonly (string Step, string Title, string Description)[] Steps =
        {
            ("S", "Dừng lại", "Trước khi tin hay chia sẻ, hãy dừng lại và kiểm tra cảm xúc của bạn."),
            ("I", "Kiểm tra nguồn", "Bạn có biết gì về nguồn thông tin này không? Nó có đáng tin không?"),
            ("F", "Tìm nguồn tốt hơn", "Xem các nguồn uy tín khác có đưa tin tương tự không."),
            ("T", "Truy nguyên gốc", "Tìm lại ngữ cảnh và nguồn gốc ban đầu của thông tin hoặc hình ảnh."),
        };

        private GameObject _panel;
        private bool _isExpanded;

        public static SIFTToolkitPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("SIFTToolkitView", parent, short.MaxValue - 20);
            root.AddComponent<CanvasGroup>();

            var iconObject = new GameObject("ToolkitIcon", typeof(RectTransform), typeof(Image), typeof(Button));
            iconObject.transform.SetParent(root.transform, false);
            RectTransform iconRect = (RectTransform)iconObject.transform;
            iconRect.anchorMin = iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 1f);
            iconRect.sizeDelta = new Vector2(96f, 96f);
            iconRect.anchoredPosition = new Vector2(24f, -24f);
            iconObject.GetComponent<Image>().color = new Color(0.15f, 0.35f, 0.6f, 0.9f);
            RuntimeUIFactory.Stretch(RuntimeUIFactory.CreateText(iconObject.transform, "Label", "SIFT", 26, TextAnchor.MiddleCenter, Color.white).rectTransform);

            var panel = new GameObject("StepsPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root.transform, false);
            RectTransform panelRect = (RectTransform)panel.transform;
            panelRect.anchorMin = panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.sizeDelta = new Vector2(560f, 340f);
            panelRect.anchoredPosition = new Vector2(24f, -132f);
            panel.GetComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 0.92f);

            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 16, 16);
            layout.spacing = 10f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            foreach ((string step, string title, string description) in Steps)
            {
                var row = new GameObject($"Step{step}", typeof(RectTransform), typeof(LayoutElement));
                row.transform.SetParent(panel.transform, false);
                row.GetComponent<LayoutElement>().preferredHeight = 70f;
                Text rowText = RuntimeUIFactory.CreateText(row.transform, "Text", $"{step} — {title}\n{description}", 20, TextAnchor.UpperLeft, Color.white);
                RuntimeUIFactory.Stretch(rowText.rectTransform);
            }

            SIFTToolkitPresenter presenter = root.AddComponent<SIFTToolkitPresenter>();
            presenter._panel = panel;
            iconObject.GetComponent<Button>().onClick.AddListener(presenter.TogglePanel);
            presenter.CollapsePanel();
            presenter.HideIcon();
            return presenter;
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
