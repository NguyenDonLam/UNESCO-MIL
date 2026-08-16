using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Shown when a major scene (a "case") wraps up, before moving on to the next one.</summary>
    public sealed class EndOfSceneCaseFilePresenter : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Text _caseNameText;
        private Text _lessonText;
        private Text _communityOutcomeText;
        private Transform _snapshotContainer;
        private Action _onContinue;

        public static EndOfSceneCaseFilePresenter Create(Transform parent)
        {
            GameObject root = RuntimeUIFactory.CreateOverlayCanvas("EndOfSceneCaseFileView", parent, short.MaxValue - 8);
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            RuntimeUIFactory.CreateDimBackground(root.transform, 0.85f);

            var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(root.transform, false);
            RectTransform cardRect = (RectTransform)card.transform;
            cardRect.anchorMin = cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(860f, 620f);
            card.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.11f, 0.98f);

            Text caseNameText = RuntimeUIFactory.CreateText(card.transform, "CaseName", string.Empty, 36, TextAnchor.MiddleCenter, Color.white);
            caseNameText.rectTransform.SetInsets(30f, 60f, 30f, 30f);

            Text lessonText = RuntimeUIFactory.CreateText(card.transform, "Lesson", string.Empty, 24, TextAnchor.UpperLeft, new Color(0.9f, 0.9f, 0.9f));
            lessonText.rectTransform.SetInsets(110f, 70f, 40f, 40f);

            Text communityOutcomeText = RuntimeUIFactory.CreateText(card.transform, "CommunityOutcome", string.Empty, 24, TextAnchor.UpperLeft, new Color(0.75f, 0.85f, 1f));
            communityOutcomeText.rectTransform.SetInsets(190f, 70f, 40f, 40f);

            var snapshotContainer = new GameObject("Snapshot", typeof(RectTransform));
            snapshotContainer.transform.SetParent(card.transform, false);
            ((RectTransform)snapshotContainer.transform).SetInsets(280f, 240f, 40f, 40f);
            var layout = snapshotContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;

            Button continueButton = RuntimeUIFactory.CreateButton(card.transform, "ContinueButton", "Sang hồ sơ tiếp theo", new Color(0.2f, 0.4f, 0.65f), 26);
            RectTransform buttonRect = continueButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = buttonRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRect.pivot = new Vector2(0.5f, 0f);
            buttonRect.sizeDelta = new Vector2(360f, 64f);
            buttonRect.anchoredPosition = new Vector2(0f, 30f);

            EndOfSceneCaseFilePresenter presenter = root.AddComponent<EndOfSceneCaseFilePresenter>();
            presenter._canvasGroup = canvasGroup;
            presenter._caseNameText = caseNameText;
            presenter._lessonText = lessonText;
            presenter._communityOutcomeText = communityOutcomeText;
            presenter._snapshotContainer = snapshotContainer.transform;
            continueButton.onClick.AddListener(presenter.OnContinueClicked);
            presenter.Hide();
            return presenter;
        }

        public void Show(string caseName, string lessonText, string communityOutcomeText, IReadOnlyList<VariableChangeSummary> snapshot, Action onContinue)
        {
            _onContinue = onContinue;
            _caseNameText.text = caseName;
            _lessonText.text = lessonText;
            _communityOutcomeText.text = communityOutcomeText;
            PopulateSnapshot(snapshot);
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

        private void PopulateSnapshot(IReadOnlyList<VariableChangeSummary> snapshot)
        {
            foreach (Transform child in _snapshotContainer) Destroy(child.gameObject);
            foreach (VariableChangeSummary entry in snapshot)
                RuntimeUIFactory.CreateText(_snapshotContainer, entry.VariableName, $"{entry.VariableName}: {entry.Delta}", 22, TextAnchor.MiddleLeft, new Color(0.85f, 0.85f, 0.85f));
        }

        private void OnContinueClicked()
        {
            Hide();
            _onContinue?.Invoke();
        }
    }
}
