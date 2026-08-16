using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Persistent collapsible toolkit icon shown in the corner during gameplay. Click to reopen the SIFT introduction card.</summary>
    public sealed class SIFTToolkitPresenter : MonoBehaviour
    {
        private static readonly Color BorderCyan = new(0.302f, 0.851f, 0.902f);
        private static readonly Color IconFill = new(0.078f, 0.110f, 0.192f);
        private static readonly Color NotificationDotColor = new(0.878f, 0.278f, 0.239f);

        private const float IconSize = 96f;

        private Action _onIconClicked;

        public static SIFTToolkitPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("SIFTToolkitView", parent, short.MaxValue - 20);
            root.AddComponent<CanvasGroup>();
            Font pixelFont = RuntimeUIFactory.PixelFont;

            var iconObject = new GameObject("ToolkitIcon", typeof(RectTransform), typeof(Image), typeof(Button));
            iconObject.transform.SetParent(root.transform, false);
            RectTransform iconRect = (RectTransform)iconObject.transform;
            iconRect.SetTopLeft(24f, 24f, new Vector2(IconSize, IconSize));
            Image border = iconObject.GetComponent<Image>();
            border.color = BorderCyan;

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(iconObject.transform, false);
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(4f, 4f);
            fillRect.offsetMax = new Vector2(-4f, -4f);
            fillObject.GetComponent<Image>().color = IconFill;

            BuildMagnifyingGlass(fillObject.transform);

            Text label = RuntimeUIFactory.CreateText(fillObject.transform, "Label", "SIFT", 15, TextAnchor.LowerCenter, Color.white, pixelFont);
            RectTransform labelRect = (RectTransform)label.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(0f, 6f);
            labelRect.offsetMax = new Vector2(0f, -8f);

            BuildNotificationDot(iconObject.transform);

            Button iconButton = iconObject.GetComponent<Button>();
            iconButton.targetGraphic = border;

            SIFTToolkitPresenter presenter = root.AddComponent<SIFTToolkitPresenter>();
            iconButton.onClick.AddListener(presenter.OnIconClicked);
            presenter.HideIcon();
            return presenter;
        }

        private static void BuildMagnifyingGlass(Transform parent)
        {
            Sprite circleSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");

            var lensObject = new GameObject("LensOuter", typeof(RectTransform), typeof(Image));
            lensObject.transform.SetParent(parent, false);
            RectTransform lensRect = (RectTransform)lensObject.transform;
            lensRect.anchorMin = lensRect.anchorMax = new Vector2(0.5f, 0.62f);
            lensRect.sizeDelta = new Vector2(34f, 34f);
            Image lensOuter = lensObject.GetComponent<Image>();
            lensOuter.sprite = circleSprite;
            lensOuter.color = BorderCyan;

            var lensInnerObject = new GameObject("LensInner", typeof(RectTransform), typeof(Image));
            lensInnerObject.transform.SetParent(lensObject.transform, false);
            RectTransform lensInnerRect = (RectTransform)lensInnerObject.transform;
            lensInnerRect.anchorMin = Vector2.zero;
            lensInnerRect.anchorMax = Vector2.one;
            lensInnerRect.offsetMin = new Vector2(6f, 6f);
            lensInnerRect.offsetMax = new Vector2(-6f, -6f);
            Image lensInner = lensInnerObject.GetComponent<Image>();
            lensInner.sprite = circleSprite;
            lensInner.color = IconFill;

            var handleObject = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handleObject.transform.SetParent(parent, false);
            RectTransform handleRect = (RectTransform)handleObject.transform;
            handleRect.anchorMin = handleRect.anchorMax = new Vector2(0.5f, 0.62f);
            handleRect.pivot = new Vector2(0.5f, 1f);
            handleRect.sizeDelta = new Vector2(5f, 16f);
            handleRect.anchoredPosition = new Vector2(11f, -11f);
            handleRect.localRotation = Quaternion.Euler(0f, 0f, -45f);
            handleObject.GetComponent<Image>().color = BorderCyan;
        }

        private static void BuildNotificationDot(Transform iconTransform)
        {
            Sprite circleSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            var dotObject = new GameObject("NotificationDot", typeof(RectTransform), typeof(Image));
            dotObject.transform.SetParent(iconTransform, false);
            RectTransform dotRect = (RectTransform)dotObject.transform;
            dotRect.anchorMin = dotRect.anchorMax = new Vector2(1f, 1f);
            dotRect.pivot = new Vector2(0.5f, 0.5f);
            dotRect.sizeDelta = new Vector2(16f, 16f);
            dotRect.anchoredPosition = new Vector2(-4f, -4f);
            Image dot = dotObject.GetComponent<Image>();
            dot.sprite = circleSprite;
            dot.color = NotificationDotColor;
        }

        /// <summary>Binds the action to invoke when the player clicks the toolkit icon (typically reopens the SIFT introduction card).</summary>
        public void Bind(Action onIconClicked) => _onIconClicked = onIconClicked;

        public void ShowIcon() => gameObject.SetActive(true);
        public void HideIcon() => gameObject.SetActive(false);

        private void OnIconClicked() => _onIconClicked?.Invoke();
    }
}
