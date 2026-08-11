using Bedrot.Narrative.Domain;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bedrot.Narrative.Presentation
{
    public sealed class CharacterSpritePresenter : MonoBehaviour
    {
        [FormerlySerializedAs("adapter")]
        [SerializeField] private UnityCharacterSpriteAdapter leftAdapter;
        [SerializeField] private UnityCharacterSpriteAdapter rightAdapter;
        private string _leftSpeakerId;
        private string _rightSpeakerId;
        private CharacterSpriteSlot _lastUsedSlot = CharacterSpriteSlot.Right;

        public void PresentBeat(NarrativeBeat beat)
        {
            if (!beat.SpeakerId.HasValue)
            {
                Hide();
                return;
            }

            string appearanceCue = !string.IsNullOrWhiteSpace(beat.AnimationCue)
                ? beat.AnimationCue
                : beat.CharacterSpriteCue;

            CharacterSpriteSlot targetSlot = ResolveSlot(beat);
            UnityCharacterSpriteAdapter targetAdapter = targetSlot == CharacterSpriteSlot.Right
                ? rightAdapter
                : leftAdapter;
            PresentAppearance(targetAdapter, appearanceCue);
        }

        public void Hide()
        {
            leftAdapter?.HideSprite();
            rightAdapter?.HideSprite();
        }

        private CharacterSpriteSlot ResolveSlot(NarrativeBeat beat)
        {
            string speakerId = beat.SpeakerId.Value.Value;
            CharacterSpriteSlot slot = beat.SpriteSlot;

            if (slot == CharacterSpriteSlot.Auto)
            {
                if (_leftSpeakerId == speakerId) slot = CharacterSpriteSlot.Left;
                else if (_rightSpeakerId == speakerId) slot = CharacterSpriteSlot.Right;
                else if (string.IsNullOrEmpty(_leftSpeakerId)) slot = CharacterSpriteSlot.Left;
                else if (string.IsNullOrEmpty(_rightSpeakerId)) slot = CharacterSpriteSlot.Right;
                else slot = _lastUsedSlot == CharacterSpriteSlot.Left
                    ? CharacterSpriteSlot.Right
                    : CharacterSpriteSlot.Left;
            }

            AssignSpeakerToSlot(speakerId, slot);
            _lastUsedSlot = slot;
            return slot;
        }

        private void AssignSpeakerToSlot(string speakerId, CharacterSpriteSlot slot)
        {
            if (slot == CharacterSpriteSlot.Right)
            {
                if (_leftSpeakerId == speakerId)
                {
                    leftAdapter?.HideSprite();
                    _leftSpeakerId = null;
                }
                _rightSpeakerId = speakerId;
                return;
            }

            if (_rightSpeakerId == speakerId)
            {
                rightAdapter?.HideSprite();
                _rightSpeakerId = null;
            }
            _leftSpeakerId = speakerId;
        }

        private static void PresentAppearance(UnityCharacterSpriteAdapter adapter, string appearanceCue)
        {
            if (adapter == null) return;
            if (string.IsNullOrWhiteSpace(appearanceCue)) adapter.HideSprite();
            else adapter.ShowAppearance(appearanceCue);
        }
    }
}
