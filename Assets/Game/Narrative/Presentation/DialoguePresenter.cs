using System;
using System.Collections;
using Bedrot.Narrative.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class DialoguePresenter : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        [Min(1f)]
        [SerializeField] private float charactersPerSecond = 45f;
        private Coroutine _revealCoroutine;
        private string _fullText = string.Empty;
        private Action _advanceRequested;

        public bool IsRevealing { get; private set; }

        public void BindAdvanceRequest(Action advanceRequested) => _advanceRequested = advanceRequested;

        public void PresentBeat(NarrativeBeat beat)
        {
            StopReveal();
            if (speakerNameText != null)
            {
                speakerNameText.text = beat.SpeakerId?.Value ?? string.Empty;
                speakerNameText.gameObject.SetActive(beat.SpeakerId.HasValue);
            }

            _fullText = beat.Text ?? string.Empty;
            if (dialogueText == null) return;
            if (_fullText.Length == 0)
            {
                dialogueText.text = string.Empty;
                return;
            }

            dialogueText.text = string.Empty;
            IsRevealing = true;
            _revealCoroutine = StartCoroutine(RevealText());
        }

        public bool CompleteRevealImmediately()
        {
            if (!IsRevealing) return false;
            StopReveal();
            if (dialogueText != null) dialogueText.text = _fullText;
            return true;
        }

        public void OnPointerClick(PointerEventData eventData) => _advanceRequested?.Invoke();

        public void Clear()
        {
            StopReveal();
            _fullText = string.Empty;
            if (speakerNameText != null) speakerNameText.text = string.Empty;
            if (dialogueText != null) dialogueText.text = string.Empty;
        }

        private IEnumerator RevealText()
        {
            int visibleCharacterCount = 0;
            float revealProgress = 0f;
            while (visibleCharacterCount < _fullText.Length)
            {
                revealProgress += charactersPerSecond * Time.unscaledDeltaTime;
                int nextCharacterCount = Mathf.Min(_fullText.Length, Mathf.FloorToInt(revealProgress));
                if (nextCharacterCount > visibleCharacterCount)
                {
                    visibleCharacterCount = nextCharacterCount;
                    dialogueText.text = _fullText.Substring(0, visibleCharacterCount);
                }
                yield return null;
            }

            dialogueText.text = _fullText;
            IsRevealing = false;
            _revealCoroutine = null;
        }

        private void StopReveal()
        {
            if (_revealCoroutine != null) StopCoroutine(_revealCoroutine);
            _revealCoroutine = null;
            IsRevealing = false;
        }

        private void OnDisable() => StopReveal();
    }
}
