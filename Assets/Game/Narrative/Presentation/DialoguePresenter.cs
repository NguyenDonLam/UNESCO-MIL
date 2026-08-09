using Bedrot.Narrative.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class DialoguePresenter : MonoBehaviour
    {
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        public void PresentBeat(NarrativeBeat beat)
        {
            if (speakerNameText != null)
            {
                speakerNameText.text = beat.SpeakerId?.Value ?? string.Empty;
                speakerNameText.gameObject.SetActive(beat.SpeakerId.HasValue);
            }
            if (dialogueText != null) dialogueText.text = beat.Text;
        }
        public void Clear() { if (speakerNameText != null) speakerNameText.text = string.Empty; if (dialogueText != null) dialogueText.text = string.Empty; }
    }
}
