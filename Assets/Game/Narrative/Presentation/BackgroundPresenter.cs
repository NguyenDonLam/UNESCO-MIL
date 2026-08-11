using Bedrot.Narrative.Domain;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    public sealed class BackgroundPresenter : MonoBehaviour
    {
        [SerializeField] private UnityBackgroundAdapter adapter;
        public void PresentBeat(NarrativeBeat beat)
        {
            if (!string.IsNullOrWhiteSpace(beat.BackgroundCue)) adapter?.ShowBackground(beat.BackgroundCue);
        }
        public void Hide() => adapter?.HideBackground();
    }
}
