using System;
using System.Collections.Generic;
using Bedrot.Narrative.Application;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Shown after the final ending when the player chooses "Share Your Experience". Collects an anonymous misinformation situation.</summary>
    public sealed class CommunityContributionPresenter : MonoBehaviour
    {
        private static readonly string[] SituationTypes =
        {
            "Thông tin sai lệch", "Nội dung thiếu ngữ cảnh", "Hình ảnh/video bị chỉnh sửa",
            "Xâm phạm quyền riêng tư", "Lừa đảo", "Nội dung do AI tạo ra", "Khác",
        };

        private static readonly Color BorderCyan = new(0.302f, 0.851f, 0.902f);
        private static readonly Color CardFill = new(0.078f, 0.110f, 0.192f);
        private static readonly Color GapColor = new(0.039f, 0.055f, 0.102f);
        private static readonly Color TitleBarFill = new(0.106f, 0.165f, 0.271f);
        private static readonly Color TitleTextColor = new(0.741f, 0.953f, 0.973f);
        private static readonly Color TitleShadowColor = new(0.051f, 0.227f, 0.247f);
        private static readonly Color BodyTextColor = new(0.780f, 0.816f, 0.878f);
        private static readonly Color TagFillDefault = new(0.059f, 0.094f, 0.188f);
        private static readonly Color TagTextDefault = new(0.498f, 0.902f, 0.937f);
        private static readonly Color TagTextSelected = new(0.039f, 0.078f, 0.125f);
        private static readonly Color InputTextColor = new(0.894f, 0.918f, 0.957f);
        private static readonly Color InputPlaceholderColor = new(0.353f, 0.392f, 0.471f);
        private static readonly Color DividerColor = new(0.180f, 0.255f, 0.376f);
        private static readonly Color ButtonPrimaryBorder = new(0.420f, 0.796f, 0.467f);
        private static readonly Color ButtonPrimaryText = new(0.624f, 0.941f, 0.675f);
        private static readonly Color ButtonSecondaryBorder = new(0.541f, 0.576f, 0.651f);
        private static readonly Color ButtonSecondaryText = new(0.722f, 0.753f, 0.824f);
        private static readonly Color StatusTextColor = new(0.353f, 0.392f, 0.471f);

        private const float CardWidth = 820f;
        private const float CardHeight = 970f;
        private const float ContentMargin = 30f;
        private const float TagRowHeight = 68f;
        private const float TagRowSpacing = 12f;

        private CanvasGroup _canvasGroup;
        private InputField _descriptionField;
        private Toggle _consentToggle;
        private Text _statusText;
        private readonly List<(Button Button, Image Border, Image Fill, Text Label, string SituationType)> _tagButtons = new();
        private string _selectedSituationType;
        private Action<SubmitCommunityContributionCommand> _onSubmit;
        private Action _onSkip;

        public static CommunityContributionPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("CommunityContributionView", parent, short.MaxValue - 6);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            RuntimeUIFactory.CreateDimBackground(root.transform, 0.85f);

            Font pixelFont = RuntimeUIFactory.PixelFont;
            Font bodyFont = RuntimeUIFactory.SecondaryFont;

            (RectTransform frame, RectTransform fill) = RuntimeUIFactory.CreateStretchPanel(
                root.transform, "Card", new RectOffset(48, 48, 48, 48), BorderCyan, CardFill, 8f);
            RuntimeUIFactory.CreateCornerBlocks(frame, new Vector2(20f, 44f), 18f, BorderCyan, GapColor, 5f);
            Transform content = RuntimeUIFactory.CreateCenteredColumn(fill, "Column", new Vector2(CardWidth - 12f, CardHeight - 12f));

            BuildTitleBar(content, pixelFont);
            BuildSubtitle(content, bodyFont);

            CommunityContributionPresenter presenter = root.AddComponent<CommunityContributionPresenter>();
            presenter._canvasGroup = canvasGroup;
            BuildTagGrid(content, pixelFont, presenter);
            presenter._descriptionField = BuildDescriptionField(content, bodyFont);
            presenter._consentToggle = BuildConsentRow(content, bodyFont);
            BuildDivider(content);
            (Button submitButton, Button skipButton, Text statusText) = BuildButtons(content, pixelFont);
            presenter._statusText = statusText;

            submitButton.onClick.AddListener(presenter.OnSubmitClicked);
            skipButton.onClick.AddListener(presenter.OnSkipClicked);
            presenter.Hide();
            return presenter;
        }

        private static void BuildTitleBar(Transform content, Font pixelFont)
        {
            var titleBar = new GameObject("TitleBar", typeof(RectTransform), typeof(Image));
            titleBar.transform.SetParent(content, false);
            ((RectTransform)titleBar.transform).SetInsets(0f, 88f, 0f, 0f);
            titleBar.GetComponent<Image>().color = TitleBarFill;

            Text title = RuntimeUIFactory.CreateText(titleBar.transform, "Title", "CHIA SẺ TRẢI NGHIỆM CỦA BẠN", 30,
                TextAnchor.MiddleCenter, TitleTextColor, pixelFont);
            RuntimeUIFactory.Stretch(title.rectTransform);
            Shadow titleShadow = title.gameObject.AddComponent<Shadow>();
            titleShadow.effectColor = TitleShadowColor;
            titleShadow.effectDistance = new Vector2(0f, -3f);

            var underline = new GameObject("Underline", typeof(RectTransform), typeof(Image));
            underline.transform.SetParent(content, false);
            ((RectTransform)underline.transform).SetInsets(88f, 6f, 0f, 0f);
            underline.GetComponent<Image>().color = BorderCyan;
        }

        private static void BuildSubtitle(Transform content, Font bodyFont)
        {
            Text subtitle = RuntimeUIFactory.CreateText(content, "Subtitle",
                "Bạn từng gặp một tình huống thông tin sai lệch? Hãy chia sẻ (ẩn danh) để giúp người khác.",
                20, TextAnchor.UpperCenter, BodyTextColor, bodyFont);
            subtitle.rectTransform.SetInsets(112f, 76f, ContentMargin, ContentMargin);
        }

        private static void BuildTagGrid(Transform content, Font pixelFont, CommunityContributionPresenter presenter)
        {
            var gridContainer = new GameObject("SituationTypes", typeof(RectTransform));
            gridContainer.transform.SetParent(content, false);
            ((RectTransform)gridContainer.transform).SetInsets(204f, 316f, ContentMargin, ContentMargin);

            float columnWidth = (CardWidth - 12f - 2f * ContentMargin - TagRowSpacing) * 0.5f;

            for (int index = 0; index < SituationTypes.Length; index++)
            {
                bool isWide = index == SituationTypes.Length - 1;
                int row = index / 2;
                int column = index % 2;

                var (button, border, fill, label) = RuntimeUIFactory.CreateTagButton(gridContainer.transform, $"Tag_{index}", SituationTypes[index], pixelFont, 16);
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                float top = row * (TagRowHeight + TagRowSpacing);
                if (isWide)
                {
                    buttonRect.SetInsets(top, TagRowHeight, 0f, 0f);
                }
                else
                {
                    float left = column * (columnWidth + TagRowSpacing);
                    buttonRect.anchorMin = new Vector2(0f, 1f);
                    buttonRect.anchorMax = new Vector2(0f, 1f);
                    buttonRect.pivot = new Vector2(0f, 1f);
                    buttonRect.sizeDelta = new Vector2(columnWidth, TagRowHeight);
                    buttonRect.anchoredPosition = new Vector2(left, -top);
                }

                border.color = BorderCyan;
                fill.color = TagFillDefault;
                label.color = TagTextDefault;

                string situationType = SituationTypes[index];
                var entry = (button, border, fill, label, situationType);
                presenter._tagButtons.Add(entry);
                button.onClick.AddListener(() => presenter.OnTagClicked(situationType));
            }
        }

        private static InputField BuildDescriptionField(Transform content, Font bodyFont)
        {
            (RectTransform frame, InputField field) = RuntimeUIFactory.CreateOutlinedInputField(content, "DescriptionField",
                "Mô tả tình huống (không nêu tên thật)...", BorderCyan, TagFillDefault, InputTextColor, InputPlaceholderColor, bodyFont, 20);
            frame.SetInsets(540f, 180f, ContentMargin, ContentMargin);
            return field;
        }

        private static Toggle BuildConsentRow(Transform content, Font bodyFont)
        {
            Toggle toggle = RuntimeUIFactory.CreateCheckboxRow(content, "ConsentRow", "Tôi đồng ý chia sẻ nội dung này ẩn danh",
                24f, bodyFont, 18, BorderCyan, TagFillDefault, BorderCyan, BodyTextColor);
            ((RectTransform)toggle.transform).SetInsets(742f, 38f, ContentMargin, ContentMargin);
            return toggle;
        }

        private static void BuildDivider(Transform content)
        {
            var divider = new GameObject("Divider", typeof(RectTransform), typeof(Image));
            divider.transform.SetParent(content, false);
            ((RectTransform)divider.transform).SetInsets(800f, 3f, ContentMargin, ContentMargin);
            divider.GetComponent<Image>().color = DividerColor;
        }

        private static (Button Submit, Button Skip, Text Status) BuildButtons(Transform content, Font pixelFont)
        {
            var buttonRow = new GameObject("Buttons", typeof(RectTransform));
            buttonRow.transform.SetParent(content, false);
            ((RectTransform)buttonRow.transform).SetInsets(821f, 72f, ContentMargin, ContentMargin);

            float rowWidth = CardWidth - 12f - 2f * ContentMargin;
            const float gap = 16f;
            float buttonWidth = (rowWidth - gap) * 0.5f;

            Button submitButton = RuntimeUIFactory.CreateOutlinedButton(buttonRow.transform, "SubmitButton", "GỬI ẨN DANH", TagFillDefault, ButtonPrimaryBorder, pixelFont, 20);
            RectTransform submitRect = submitButton.GetComponent<RectTransform>();
            submitRect.anchorMin = new Vector2(0f, 0f); submitRect.anchorMax = new Vector2(0f, 1f);
            submitRect.pivot = new Vector2(0f, 0.5f);
            submitRect.sizeDelta = new Vector2(buttonWidth, 0f);
            submitRect.anchoredPosition = new Vector2(0f, 0f);
            submitButton.GetComponentInChildren<Text>().color = ButtonPrimaryText;

            Button skipButton = RuntimeUIFactory.CreateOutlinedButton(buttonRow.transform, "SkipButton", "BỎ QUA", TagFillDefault, ButtonSecondaryBorder, pixelFont, 20);
            RectTransform skipRect = skipButton.GetComponent<RectTransform>();
            skipRect.anchorMin = new Vector2(1f, 0f); skipRect.anchorMax = new Vector2(1f, 1f);
            skipRect.pivot = new Vector2(1f, 0.5f);
            skipRect.sizeDelta = new Vector2(buttonWidth, 0f);
            skipRect.anchoredPosition = new Vector2(0f, 0f);
            skipButton.GetComponentInChildren<Text>().color = ButtonSecondaryText;

            Text status = RuntimeUIFactory.CreateText(content, "StatusLine", string.Empty, 16, TextAnchor.MiddleCenter, StatusTextColor, pixelFont);
            status.rectTransform.SetInsets(909f, 26f, ContentMargin, ContentMargin);

            return (submitButton, skipButton, status);
        }

        public void Bind(Action<SubmitCommunityContributionCommand> onSubmit, Action onSkip)
        {
            _onSubmit = onSubmit;
            _onSkip = onSkip;
        }

        public void Show()
        {
            _selectedSituationType = null;
            foreach (var tag in _tagButtons)
            {
                tag.Fill.color = TagFillDefault;
                tag.Label.color = TagTextDefault;
            }
            _descriptionField.text = string.Empty;
            _consentToggle.isOn = false;
            _statusText.text = string.Empty;
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

        private void OnTagClicked(string situationType)
        {
            _selectedSituationType = _selectedSituationType == situationType ? null : situationType;
            foreach (var tag in _tagButtons)
            {
                bool selected = tag.SituationType == _selectedSituationType;
                tag.Fill.color = selected ? BorderCyan : TagFillDefault;
                tag.Label.color = selected ? TagTextSelected : TagTextDefault;
            }
        }

        private void OnSubmitClicked()
        {
            if (!_consentToggle.isOn)
            {
                _statusText.text = "VUI LÒNG ĐÁNH DẤU ĐỒNG Ý TRƯỚC";
                return;
            }

            var command = new SubmitCommunityContributionCommand(_selectedSituationType ?? string.Empty, _descriptionField.text, _consentToggle.isOn);
            Hide();
            _onSubmit?.Invoke(command);
        }

        private void OnSkipClicked()
        {
            Hide();
            _onSkip?.Invoke();
        }
    }
}
