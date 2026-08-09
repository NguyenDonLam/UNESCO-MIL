using System;
using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class ChoicePresenter : MonoBehaviour
    {
        [SerializeField] private Button choiceButtonPrefab;
        [SerializeField] private Transform choiceContainer;
        private readonly List<Button> _buttons = new();

        public void PresentChoices(IReadOnlyList<NarrativeChoice> choices, Action<ChoiceId> onSelected)
        {
            EnsureButtons(choices.Count);
            for (int index = 0; index < _buttons.Count; index++)
            {
                Button button = _buttons[index]; button.onClick.RemoveAllListeners();
                bool active = index < choices.Count; button.gameObject.SetActive(active);
                if (!active) continue;
                NarrativeChoice choice = choices[index];
                Text label = button.GetComponentInChildren<Text>(true);
                if (label != null) label.text = choice.Text;
                button.onClick.AddListener(() => onSelected(choice.Id));
            }
            gameObject.SetActive(choices.Count > 0);
        }

        public void HideChoices()
        {
            foreach (Button button in _buttons) { button.onClick.RemoveAllListeners(); button.gameObject.SetActive(false); }
            gameObject.SetActive(false);
        }

        private void EnsureButtons(int count)
        {
            if (choiceContainer == null) choiceContainer = transform;
            if (_buttons.Count == 0) _buttons.AddRange(choiceContainer.GetComponentsInChildren<Button>(true));
            while (_buttons.Count < count)
            {
                if (choiceButtonPrefab == null) throw new InvalidOperationException("ChoicePresenter needs a button prefab when it cannot reuse enough child buttons.");
                _buttons.Add(Instantiate(choiceButtonPrefab, choiceContainer));
            }
        }
    }
}
