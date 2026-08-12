using System;
using System.Collections;
using Bedrot.Narrative.Application;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Bedrot.Narrative.Infrastructure
{
    public sealed class UnitySceneLoadingAdapter : MonoBehaviour, IUnitySceneLoadingAdapter
    {
        public void LoadSceneAsync(string sceneName, Action onLoaded) => StartCoroutine(LoadSceneCoroutine(sceneName, onLoaded));
        private static IEnumerator LoadSceneCoroutine(string sceneName, Action onLoaded)
        {
#if UNITY_EDITOR
            string editorScenePath = $"Assets/Scenes/{sceneName}.unity";
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(
                editorScenePath,
                new LoadSceneParameters(LoadSceneMode.Single));
#else
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
#endif
            if (operation == null) throw new InvalidOperationException($"Unity could not start loading scene '{sceneName}'.");
            while (!operation.isDone) yield return null;
            onLoaded?.Invoke();
        }
    }
}
