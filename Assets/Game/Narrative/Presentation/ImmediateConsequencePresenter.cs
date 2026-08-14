using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Shown right after a choice, before the next scene renders, summarizing what just changed.</summary>
    public sealed class ImmediateConsequencePresenter : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Transform _changesContainer;
        private Action _onContinue;

        public static ImmediateConsequencePresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("ImmediateConsequenceView", parent, short.MaxValue - 8);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            RuntimeUIFactory.CreateDimBackground(root.transform, 0.75f);

            var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(root.transform, false);
            RectTransform cardRect = (RectTransform)card.transform;
            cardRect.anchorMin = cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(700f, 480f);
            card.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.12f, 0.97f);

            RuntimeUIFactory.CreateText(card.transform, "Title", "Hệ quả tức thời", 34, TextAnchor.MiddleCenter, Color.white)
                .rectTransform.SetInsets(30f, 60f, 30f, 30f);

            var changesContainer = new GameObject("Changes", typeof(RectTransform));
            changesContainer.transform.SetParent(card.transform, false);
            RectTransform changesRect = (RectTransform)changesContainer.transform;
            changesRect.SetInsets(110f, 260f, 40f, 40f);
            var layout = changesContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;

            Button continueButton = RuntimeUIFactory.CreateButton(card.transform, "ContinueButton", "Tiếp tục", new Color(0.2f, 0.4f, 0.65f), 28);
            RectTransform buttonRect = continueButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.sizeDelta = new Vector2(280f, 64f);
            buttonRect.anchoredPosition = new Vector2(0f, 30f);

            ImmediateConsequencePresenter presenter = root.AddComponent<ImmediateConsequencePresenter>();
            presenter._canvasGroup = canvasGroup;
            presenter._changesContainer = changesContainer.transform;
            continueButton.onClick.AddListener(presenter.OnContinueClicked);
            presenter.Hide();
            return presenter;
        }

        public void Show(IReadOnlyList<VariableChangeSummary> changes, Action onContinue)
        {
            _onContinue = onContinue;
            PopulateChanges(changes);
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

        private void PopulateChanges(IReadOnlyList<VariableChangeSummary> changes)
        {
            foreach (Transform child in _changesContainer) Destroy(child.gameObject);
            foreach (VariableChangeSummary change in changes)
            {
                string arrow = change.IsPositive ? "▲" : "▼";
                string sign = change.Delta >= 0 ? "+" : string.Empty;
                Color color = change.IsPositive ? new Color(0.35f, 0.85f, 0.4f) : new Color(0.9f, 0.35f, 0.35f);
                RuntimeUIFactory.CreateText(_changesContainer, change.VariableName, $"{change.VariableName}  {arrow} {sign}{change.Delta}", 24, TextAnchor.MiddleLeft, color);
            }
        }

        private void OnContinueClicked()
        {
            Hide();
            _onContinue?.Invoke();
        }
    }
}
