using Bedrot.Narrative.Domain;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    public sealed class CharacterSpritePresenter : MonoBehaviour
    {
        [SerializeField] private UnityCharacterSpriteAdapter adapter;
        public void PresentBeat(NarrativeBeat beat) { if (!string.IsNullOrWhiteSpace(beat.CharacterSpriteCue)) adapter.ShowSprite(beat.CharacterSpriteCue); }
        public void Hide() => adapter?.HideSprite();
    }

    public sealed class BackgroundPresenter : MonoBehaviour
    {
        [SerializeField] private UnityBackgroundAdapter adapter;
        public void PresentBeat(NarrativeBeat beat) { if (!string.IsNullOrWhiteSpace(beat.BackgroundCue)) adapter.ShowBackground(beat.BackgroundCue); }
        public void Hide() => adapter?.HideBackground();
    }

    public sealed class TransitionPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        public void SetVisible(bool visible) { if (canvasGroup != null) { canvasGroup.alpha = visible ? 1f : 0f; canvasGroup.blocksRaycasts = visible; } }
    }
}
