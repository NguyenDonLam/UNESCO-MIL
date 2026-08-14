#if UNITY_EDITOR
using Bedrot.Narrative.Presentation;
using UnityEditor;
using UnityEngine;

namespace Bedrot.Narrative.Authoring.Editor
{
    /// <summary>Lets developers re-trigger the one-time SIFT introduction card without clearing all PlayerPrefs.</summary>
    public static class SIFTIntroDebugMenu
    {
        [MenuItem("Bedrot/Reset SIFT Intro Seen Flag")]
        public static void ResetSIFTIntroSeenFlag()
        {
            PlayerPrefs.DeleteKey(GamePresentationGateway.HasSeenSIFTIntroPrefKey);
            PlayerPrefs.Save();
            Debug.Log("The SIFT introduction card will show again on the next new game.");
        }
    }
}
#endif
