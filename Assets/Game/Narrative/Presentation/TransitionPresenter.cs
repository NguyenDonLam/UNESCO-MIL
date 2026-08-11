using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    public sealed class TransitionPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        public void SetVisible(bool visible)
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
