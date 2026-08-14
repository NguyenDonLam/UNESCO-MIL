using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>RectTransform layout helpers used by runtime-built presenters.</summary>
    internal static class RectTransformExtensions
    {
        public static void Stretch(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>Stretches horizontally and pins the top edge, giving a fixed height band from the top.</summary>
        public static void SetInsets(this RectTransform rectTransform, float top, float height, float left, float right)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -top);
            rectTransform.sizeDelta = new Vector2(-(left + right), height);
        }
    }

    /// <summary>Shared helpers for presenters that build their UI at runtime instead of via scene-authored prefabs.</summary>
    internal static class RuntimeUIFactory
    {
        public static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static void Stretch(RectTransform rectTransform) => rectTransform.Stretch();

        public static GameObject CreateOverlayCanvas(string name, Transform parent, int sortingOrder)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            if (parent != null) root.transform.SetParent(parent, false);
            Stretch((RectTransform)root.transform);

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = (short)sortingOrder;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return root;
        }

        public static Image CreateDimBackground(Transform parent, float alpha = 0.7f)
        {
            var background = new GameObject("DimBackground", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(parent, false);
            Stretch((RectTransform)background.transform);
            Image image = background.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, alpha);
            return image;
        }

        public static Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.font = DefaultFont;
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Button CreateButton(Transform parent, string name, string label, Color backgroundColor, int fontSize = 28)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image background = buttonObject.GetComponent<Image>();
            background.color = backgroundColor;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = background;

            Text label1 = CreateText(buttonObject.transform, "Label", label, fontSize, TextAnchor.MiddleCenter, Color.white);
            Stretch((RectTransform)label1.transform);
            return button;
        }

        public static Toggle CreateToggle(Transform parent, string name, string label)
        {
            var toggleObject = new GameObject(name, typeof(RectTransform), typeof(Toggle));
            toggleObject.transform.SetParent(parent, false);

            var backgroundObject = new GameObject("Background", typeof(RectTransform), typeof(Image));
            backgroundObject.transform.SetParent(toggleObject.transform, false);
            RectTransform backgroundRect = (RectTransform)backgroundObject.transform;
            backgroundRect.anchorMin = new Vector2(0f, 0.5f);
            backgroundRect.anchorMax = new Vector2(0f, 0.5f);
            backgroundRect.sizeDelta = new Vector2(32f, 32f);
            backgroundRect.anchoredPosition = new Vector2(16f, 0f);
            Image background = backgroundObject.GetComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0.9f);

            var checkObject = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            checkObject.transform.SetParent(backgroundObject.transform, false);
            Stretch((RectTransform)checkObject.transform);
            Image check = checkObject.GetComponent<Image>();
            check.color = new Color(0.2f, 0.6f, 0.3f);

            Text labelText = CreateText(toggleObject.transform, "Label", label, 24, TextAnchor.MiddleLeft, Color.white);
            RectTransform labelRect = (RectTransform)labelText.transform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(40f, 0f);
            labelRect.offsetMax = Vector2.zero;

            Toggle toggle = toggleObject.GetComponent<Toggle>();
            toggle.targetGraphic = background;
            toggle.graphic = check;
            toggle.isOn = false;
            return toggle;
        }

        public static InputField CreateInputField(Transform parent, string placeholder)
        {
            var fieldObject = new GameObject("InputField", typeof(RectTransform), typeof(Image), typeof(InputField));
            fieldObject.transform.SetParent(parent, false);
            fieldObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.95f);

            Text placeholderText = CreateText(fieldObject.transform, "Placeholder", placeholder, 22, TextAnchor.UpperLeft, new Color(0.4f, 0.4f, 0.4f));
            RectTransform placeholderRect = (RectTransform)placeholderText.transform;
            placeholderRect.anchorMin = Vector2.zero; placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10f, 6f); placeholderRect.offsetMax = new Vector2(-10f, -6f);

            Text valueText = CreateText(fieldObject.transform, "Text", string.Empty, 22, TextAnchor.UpperLeft, Color.black);
            RectTransform valueRect = (RectTransform)valueText.transform;
            valueRect.anchorMin = Vector2.zero; valueRect.anchorMax = Vector2.one;
            valueRect.offsetMin = new Vector2(10f, 6f); valueRect.offsetMax = new Vector2(-10f, -6f);

            InputField field = fieldObject.GetComponent<InputField>();
            field.textComponent = valueText;
            field.placeholder = placeholderText;
            field.lineType = InputField.LineType.MultiLineNewline;
            return field;
        }
    }
}
