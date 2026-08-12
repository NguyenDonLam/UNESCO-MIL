using System;
using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    [Serializable]
    public sealed class CharacterAppearanceCueEntry
    {
        [Tooltip("Semantic cue used by a narrative beat, for example linh.worried.")]
        public string Cue;
        [Tooltip("Static appearance, or the initial frame shown before an animation starts.")]
        public Sprite Sprite;
        [Tooltip("Optional Animator Controller. When assigned, this entry is presented as an animation.")]
        public RuntimeAnimatorController AnimationController;
        [Tooltip("Optional state to play. Leave empty to use the controller's default state.")]
        public string AnimationStateName;

        public bool HasAppearance => Sprite != null || AnimationController != null;
        public bool IsAnimated => AnimationController != null;
    }
}
