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

        private CanvasGroup _canvasGroup;
        private InputField _descriptionField;
        private Toggle _consentToggle;
        private readonly List<Toggle> _situationToggles = new();
        private string _selectedSituationType;
        private Action<SubmitCommunityContributionCommand> _onSubmit;
        private Action _onSkip;

        public static CommunityContributionPresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("CommunityContributionView", parent, short.MaxValue - 6);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            RuntimeUIFactory.CreateDimBackground(root.transform, 0.85f);

            var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(root.transform, false);
            RectTransform cardRect = (RectTransform)card.transform;
            cardRect.anchorMin = cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(900f, 760f);
            card.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.11f, 0.98f);

            RuntimeUIFactory.CreateText(card.transform, "Title", "Chia sẻ trải nghiệm của bạn", 32, TextAnchor.MiddleCenter, Color.white)
                .rectTransform.SetInsets(30f, 50f, 30f, 30f);
            RuntimeUIFactory.CreateText(card.transform, "Subtitle", "Bạn từng gặp một tình huống thông tin sai lệch? Hãy chia sẻ (ẩn danh) để giúp người khác.", 20,
                TextAnchor.UpperLeft, new Color(0.8f, 0.8f, 0.8f)).rectTransform.SetInsets(90f, 50f, 40f, 40f);

            var situationContainer = new GameObject("SituationTypes", typeof(RectTransform));
            situationContainer.transform.SetParent(card.transform, false);
            ((RectTransform)situationContainer.transform).SetInsets(150f, 140f, 40f, 40f);
            var flow = situationContainer.AddComponent<GridLayoutGroup>();
            flow.cellSize = new Vector2(260f, 56f);
            flow.spacing = new Vector2(12f, 12f);

            var toggles = new List<Toggle>();
            foreach (string situationType in SituationTypes)
                toggles.Add(RuntimeUIFactory.CreateToggle(situationContainer.transform, $"Situation_{situationType}", situationType));

            InputField descriptionField = RuntimeUIFactory.CreateInputField(card.transform, "Mô tả tình huống (không nêu tên thật)...");
            descriptionField.GetComponent<RectTransform>().SetInsets(310f, 220f, 40f, 40f);

            Toggle consentToggle = RuntimeUIFactory.CreateToggle(card.transform, "ConsentToggle", "Tôi đồng ý chia sẻ nội dung này ẩn danh");
            RectTransform consentRect = consentToggle.GetComponent<RectTransform>();
            consentRect.SetInsets(550f, 40f, 40f, 40f);

            Button submitButton = RuntimeUIFactory.CreateButton(card.transform, "SubmitButton", "Gửi ẩn danh", new Color(0.2f, 0.55f, 0.3f), 26);
            RectTransform submitRect = submitButton.GetComponent<RectTransform>();
            submitRect.anchorMin = submitRect.anchorMax = new Vector2(0.5f, 0f);
            submitRect.pivot = new Vector2(0.5f, 0f);
            submitRect.sizeDelta = new Vector2(300f, 64f);
            submitRect.anchoredPosition = new Vector2(-160f, 30f);

            Button skipButton = RuntimeUIFactory.CreateButton(card.transform, "SkipButton", "Bỏ qua", new Color(0.3f, 0.3f, 0.33f), 26);
            RectTransform skipRect = skipButton.GetComponent<RectTransform>();
            skipRect.anchorMin = skipRect.anchorMax = new Vector2(0.5f, 0f);
            skipRect.pivot = new Vector2(0.5f, 0f);
            skipRect.sizeDelta = new Vector2(220f, 64f);
            skipRect.anchoredPosition = new Vector2(180f, 30f);

            CommunityContributionPresenter presenter = root.AddComponent<CommunityContributionPresenter>();
            presenter._canvasGroup = canvasGroup;
            presenter._descriptionField = descriptionField;
            presenter._consentToggle = consentToggle;
            presenter._situationToggles.AddRange(toggles);
            for (int i = 0; i < toggles.Count; i++)
            {
                string situationType = SituationTypes[i];
                Toggle toggle = toggles[i];
                toggle.onValueChanged.AddListener(isOn => presenter.OnSituationToggleChanged(toggle, situationType, isOn));
            }
            submitButton.onClick.AddListener(presenter.OnSubmitClicked);
            skipButton.onClick.AddListener(presenter.OnSkipClicked);
            presenter.Hide();
            return presenter;
        }

        public void Bind(Action<SubmitCommunityContributionCommand> onSubmit, Action onSkip)
        {
            _onSubmit = onSubmit;
            _onSkip = onSkip;
        }

        public void Show()
        {
            _selectedSituationType = null;
            foreach (Toggle toggle in _situationToggles) toggle.SetIsOnWithoutNotify(false);
            _descriptionField.text = string.Empty;
            _consentToggle.isOn = false;
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

        private void OnSituationToggleChanged(Toggle changedToggle, string situationType, bool isOn)
        {
            if (!isOn) { if (_selectedSituationType == situationType) _selectedSituationType = null; return; }
            _selectedSituationType = situationType;
            foreach (Toggle other in _situationToggles)
                if (other != changedToggle) other.SetIsOnWithoutNotify(false);
        }

        private void OnSubmitClicked()
        {
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
