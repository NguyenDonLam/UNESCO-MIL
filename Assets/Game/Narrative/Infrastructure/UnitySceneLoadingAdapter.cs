using System;
using System.Collections;
using Bedrot.Narrative.Application;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bedrot.Narrative.Infrastructure
{
    public sealed class UnitySceneLoadingAdapter : MonoBehaviour, IUnitySceneLoadingAdapter
    {
        public void LoadSceneAsync(string sceneName, Action onLoaded) => StartCoroutine(LoadSceneCoroutine(sceneName, onLoaded));
        private static IEnumerator LoadSceneCoroutine(string sceneName, Action onLoaded)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null) throw new InvalidOperationException($"Unity could not start loading scene '{sceneName}'.");
            while (!operation.isDone) yield return null;
            onLoaded?.Invoke();
        }
    }
}
