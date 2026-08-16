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

        /// <summary>Stretches horizontally and pins the top edge, giving a fixed height band with independent left/right margins.</summary>
        public static void SetInsets(this RectTransform rectTransform, float top, float height, float left, float right)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.offsetMin = new Vector2(left, -(top + height));
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        /// <summary>Pins a fixed-size element to the top-left corner of its parent.</summary>
        public static void SetTopLeft(this RectTransform rectTransform, float top, float left, Vector2 size)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = new Vector2(left, -top);
        }
    }

    /// <summary>Shared helpers for presenters that build their UI at runtime instead of via scene-authored prefabs.</summary>
    internal static class RuntimeUIFactory
    {
        public static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        private static Font _pixelFont;
        /// <summary>The bold pixel-art display font used for headers, buttons and short labels.</summary>
        public static Font PixelFont => _pixelFont != null ? _pixelFont
            : _pixelFont = Resources.Load<Font>("Fonts/SVN-Determination Sans") ?? DefaultFont;

        private static Font _secondaryFont;
        /// <summary>The thinner terminal-style pixel font used for body copy (subtitles, descriptions, input text).</summary>
        public static Font SecondaryFont => _secondaryFont != null ? _secondaryFont
            : _secondaryFont = Resources.Load<Font>("Fonts/VT323-Regular") ?? DefaultFont;

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

        public static Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Color color, Font font = null)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.font = font != null ? font : DefaultFont;
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Button CreateButton(Transform parent, string name, string label, Color backgroundColor, int fontSize = 28, Font font = null)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image background = buttonObject.GetComponent<Image>();
            background.color = backgroundColor;
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = background;

            Text label1 = CreateText(buttonObject.transform, "Label", label, fontSize, TextAnchor.MiddleCenter, Color.white, font);
            Stretch((RectTransform)label1.transform);
            return button;
        }

        /// <summary>Creates a button with an outlined frame (border color ring, inset fill, centered label using the border color).</summary>
        public static Button CreateOutlinedButton(Transform parent, string name, string label, Color fillColor, Color borderColor, Font font, int fontSize = 28, float borderThickness = 3f)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image border = buttonObject.GetComponent<Image>();
            border.color = borderColor;

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(buttonObject.transform, false);
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(borderThickness, borderThickness);
            fillRect.offsetMax = new Vector2(-borderThickness, -borderThickness);
            fillObject.GetComponent<Image>().color = fillColor;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = border;

            Text label1 = CreateText(fillObject.transform, "Label", label, fontSize, TextAnchor.MiddleCenter, borderColor, font);
            Stretch((RectTransform)label1.transform);
            return button;
        }

        /// <summary>Creates a bordered square icon tile with a single centered letter, used by the SIFT step rows.</summary>
        public static Image CreateIconSquare(Transform parent, string name, string letter, Color fillColor, Color borderColor, float size, Font font, float borderThickness = 5f)
        {
            var borderObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            borderObject.transform.SetParent(parent, false);
            RectTransform borderRect = (RectTransform)borderObject.transform;
            borderRect.sizeDelta = new Vector2(size, size);
            Image border = borderObject.GetComponent<Image>();
            border.color = borderColor;

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(borderObject.transform, false);
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(borderThickness, borderThickness);
            fillRect.offsetMax = new Vector2(-borderThickness, -borderThickness);
            fillObject.GetComponent<Image>().color = fillColor;

            Text letterText = CreateText(fillObject.transform, "Letter", letter, Mathf.RoundToInt(size * 0.5f), TextAnchor.MiddleCenter, Color.white, font);
            Stretch((RectTransform)letterText.transform);
            return border;
        }

        /// <summary>Creates a panel frame that stretches to fill its parent with a fixed margin on each side: an outer border plus an inset fill.</summary>
        public static (RectTransform Frame, RectTransform Content) CreateStretchPanel(Transform parent, string name, RectOffset margin, Color borderColor, Color fillColor, float borderThickness = 8f)
        {
            var frameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            frameObject.transform.SetParent(parent, false);
            RectTransform frameRect = (RectTransform)frameObject.transform;
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.offsetMin = new Vector2(margin.left, margin.bottom);
            frameRect.offsetMax = new Vector2(-margin.right, -margin.top);
            frameObject.GetComponent<Image>().color = borderColor;

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(frameObject.transform, false);
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.one * borderThickness;
            fillRect.offsetMax = -Vector2.one * borderThickness;
            fillObject.GetComponent<Image>().color = fillColor;

            return (frameRect, fillRect);
        }

        /// <summary>Creates a fixed-size RectTransform centered inside its parent, useful as a readable content column inside a full-screen panel.</summary>
        public static RectTransform CreateCenteredColumn(Transform parent, string name, Vector2 size)
        {
            var columnObject = new GameObject(name, typeof(RectTransform));
            columnObject.transform.SetParent(parent, false);
            RectTransform columnRect = (RectTransform)columnObject.transform;
            columnRect.anchorMin = columnRect.anchorMax = new Vector2(0.5f, 0.5f);
            columnRect.sizeDelta = size;
            return columnRect;
        }

        /// <summary>Creates a multi-line bordered input field (border ring + inset fill + placeholder/value text). Position the returned Frame, not the Field.</summary>
        public static (RectTransform Frame, InputField Field) CreateOutlinedInputField(Transform parent, string name, string placeholder,
            Color borderColor, Color fillColor, Color textColor, Color placeholderColor, Font font, int fontSize, float borderThickness = 3f)
        {
            var borderObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            borderObject.transform.SetParent(parent, false);
            RectTransform borderRect = (RectTransform)borderObject.transform;
            borderObject.GetComponent<Image>().color = borderColor;

            var fieldObject = new GameObject("Field", typeof(RectTransform), typeof(Image), typeof(InputField));
            fieldObject.transform.SetParent(borderObject.transform, false);
            RectTransform fieldRect = (RectTransform)fieldObject.transform;
            fieldRect.anchorMin = Vector2.zero;
            fieldRect.anchorMax = Vector2.one;
            fieldRect.offsetMin = new Vector2(borderThickness, borderThickness);
            fieldRect.offsetMax = new Vector2(-borderThickness, -borderThickness);
            fieldObject.GetComponent<Image>().color = fillColor;

            Text placeholderText = CreateText(fieldObject.transform, "Placeholder", placeholder, fontSize, TextAnchor.UpperLeft, placeholderColor, font);
            RectTransform placeholderRect = (RectTransform)placeholderText.transform;
            placeholderRect.anchorMin = Vector2.zero; placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(14f, 10f); placeholderRect.offsetMax = new Vector2(-14f, -10f);

            Text valueText = CreateText(fieldObject.transform, "Text", string.Empty, fontSize, TextAnchor.UpperLeft, textColor, font);
            RectTransform valueRect = (RectTransform)valueText.transform;
            valueRect.anchorMin = Vector2.zero; valueRect.anchorMax = Vector2.one;
            valueRect.offsetMin = new Vector2(14f, 10f); valueRect.offsetMax = new Vector2(-14f, -10f);

            InputField field = fieldObject.GetComponent<InputField>();
            field.textComponent = valueText;
            field.placeholder = placeholderText;
            field.lineType = InputField.LineType.MultiLineNewline;
            return (borderRect, field);
        }

        /// <summary>Creates a selectable pill/tag button: border ring, inset fill, centered label. Caller manages the selected-state recolor.</summary>
        public static (Button Button, Image Border, Image Fill, Text Label) CreateTagButton(Transform parent, string name, string label, Font font, int fontSize = 20, float borderThickness = 3f)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            Image border = buttonObject.GetComponent<Image>();

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(buttonObject.transform, false);
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(borderThickness, borderThickness);
            fillRect.offsetMax = new Vector2(-borderThickness, -borderThickness);
            Image fill = fillObject.GetComponent<Image>();

            Text labelText = CreateText(fillObject.transform, "Label", label, fontSize, TextAnchor.MiddleCenter, Color.white, font);
            Stretch((RectTransform)labelText.transform);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = border;
            return (button, border, fill, labelText);
        }

        /// <summary>Creates a full-row clickable checkbox toggle: a bordered box (recolored on check) plus a label filling the rest of the row.</summary>
        public static Toggle CreateCheckboxRow(Transform parent, string name, string label, float boxSize, Font font, int fontSize,
            Color borderColor, Color uncheckedFill, Color checkedFill, Color labelColor, float borderThickness = 3f)
        {
            var rowObject = new GameObject(name, typeof(RectTransform), typeof(Toggle));
            rowObject.transform.SetParent(parent, false);

            var boxObject = new GameObject("Box", typeof(RectTransform), typeof(Image));
            boxObject.transform.SetParent(rowObject.transform, false);
            ((RectTransform)boxObject.transform).SetTopLeft(0f, 0f, new Vector2(boxSize, boxSize));
            Image border = boxObject.GetComponent<Image>();
            border.color = borderColor;

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(boxObject.transform, false);
            RectTransform fillRect = (RectTransform)fillObject.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(borderThickness, borderThickness);
            fillRect.offsetMax = new Vector2(-borderThickness, -borderThickness);
            Image fill = fillObject.GetComponent<Image>();
            fill.color = uncheckedFill;

            Text labelText = CreateText(rowObject.transform, "Label", label, fontSize, TextAnchor.MiddleLeft, labelColor, font);
            RectTransform labelRect = (RectTransform)labelText.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(boxSize + 14f, 0f);
            labelRect.offsetMax = Vector2.zero;

            Toggle toggle = rowObject.GetComponent<Toggle>();
            toggle.targetGraphic = border;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(isOn => fill.color = isOn ? checkedFill : uncheckedFill);
            return toggle;
        }

        /// <summary>Adds 4 small accent blocks (an outline color ring + inset fill) straddling the corners of the given frame, mirrored symmetrically.</summary>
        public static void CreateCornerBlocks(RectTransform frameRect, Vector2 blockSize, float outwardOffset, Color blockColor, Color outlineColor, float outlineThickness)
        {
            float cx = outwardOffset - blockSize.x * 0.5f;
            float cy = outwardOffset - blockSize.y * 0.5f;
            CreateCornerBlock(frameRect, new Vector2(0f, 1f), new Vector2(-cx, cy), blockSize, blockColor, outlineColor, outlineThickness);
            CreateCornerBlock(frameRect, new Vector2(1f, 1f), new Vector2(cx, cy), blockSize, blockColor, outlineColor, outlineThickness);
            CreateCornerBlock(frameRect, new Vector2(0f, 0f), new Vector2(-cx, -cy), blockSize, blockColor, outlineColor, outlineThickness);
            CreateCornerBlock(frameRect, new Vector2(1f, 0f), new Vector2(cx, -cy), blockSize, blockColor, outlineColor, outlineThickness);
        }

        private static void CreateCornerBlock(RectTransform parent, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, Color blockColor, Color outlineColor, float outlineThickness)
        {
            var outlineObject = new GameObject("CornerBlock", typeof(RectTransform), typeof(Image));
            outlineObject.transform.SetParent(parent, false);
            RectTransform outlineRect = (RectTransform)outlineObject.transform;
            outlineRect.anchorMin = outlineRect.anchorMax = anchor;
            outlineRect.pivot = new Vector2(0.5f, 0.5f);
            outlineRect.anchoredPosition = anchoredPosition;
            outlineRect.sizeDelta = size;
            outlineObject.GetComponent<Image>().color = outlineColor;

            var innerObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            innerObject.transform.SetParent(outlineObject.transform, false);
            RectTransform innerRect = (RectTransform)innerObject.transform;
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(outlineThickness, outlineThickness);
            innerRect.offsetMax = new Vector2(-outlineThickness, -outlineThickness);
            innerObject.GetComponent<Image>().color = blockColor;
        }
    }
}
