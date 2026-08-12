#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace Bedrot.Narrative.Authoring.Editor
{
    /// <summary>Keeps the active Unity 6 Build Profile able to load the game's required scenes.</summary>
    [InitializeOnLoad]
    public static class RequiredBuildScenesEditorGuard
    {
        private static readonly string[] RequiredScenePaths =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Game.unity"
        };

        static RequiredBuildScenesEditorGuard()
        {
            EditorApplication.delayCall += EnsureRequiredScenes;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("Bedrot/Repair Active Build Profile Scene List")]
        public static void EnsureRequiredScenes()
        {
            string[] missingAssets = RequiredScenePaths.Where(path => AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null).ToArray();
            if (missingAssets.Length > 0)
                throw new InvalidOperationException($"Required scene asset(s) missing: {string.Join(", ", missingAssets)}");

            EditorBuildSettingsScene[] globalScenes = RepairSceneList(EditorBuildSettings.globalScenes, out bool globalChanged);
            if (globalChanged) EditorBuildSettings.globalScenes = globalScenes;

            bool savedProfileChanged = RepairSavedBuildProfiles();

            EditorBuildSettingsScene[] activeScenes = RepairSceneList(EditorBuildSettings.scenes, out bool activeChanged);
            if (activeChanged) EditorBuildSettings.scenes = activeScenes;
            if (!globalChanged && !savedProfileChanged && !activeChanged) return;

            AssetDatabase.SaveAssets();
            Debug.Log("Repaired Build Profile scene lists: MainMenu.unity and Game.unity are enabled.");
        }

        private static bool RepairSavedBuildProfiles()
        {
            bool changed = false;
            foreach (string guid in AssetDatabase.FindAssets("t:BuildProfile", new[] { "Assets/Settings/Build Profiles" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BuildProfile profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
                if (profile == null) continue;

                EditorBuildSettingsScene[] repairedScenes = RepairSceneList(profile.scenes, out bool scenesChanged);
                if (!profile.overrideGlobalScenes || scenesChanged)
                {
                    profile.overrideGlobalScenes = true;
                    profile.scenes = repairedScenes;
                    EditorUtility.SetDirty(profile);
                    changed = true;
                }
            }

            return changed;
        }

        private static EditorBuildSettingsScene[] RepairSceneList(
            EditorBuildSettingsScene[] currentScenes,
            out bool changed)
        {
            currentScenes ??= Array.Empty<EditorBuildSettingsScene>();
            var scenesByPath = new Dictionary<string, EditorBuildSettingsScene>(StringComparer.OrdinalIgnoreCase);
            foreach (EditorBuildSettingsScene scene in currentScenes.Where(scene => !string.IsNullOrWhiteSpace(scene.path)))
                scenesByPath[scene.path] = scene;

            changed = false;
            foreach (string requiredPath in RequiredScenePaths)
            {
                if (scenesByPath.TryGetValue(requiredPath, out EditorBuildSettingsScene existing) && existing.enabled)
                    continue;

                scenesByPath[requiredPath] = new EditorBuildSettingsScene(requiredPath, true);
                changed = true;
            }

            if (!changed) return currentScenes;

            var repairedScenes = new List<EditorBuildSettingsScene>();
            repairedScenes.AddRange(RequiredScenePaths.Select(path => scenesByPath[path]));
            repairedScenes.AddRange(currentScenes.Where(scene => !RequiredScenePaths.Contains(scene.path, StringComparer.OrdinalIgnoreCase)));
            return repairedScenes.ToArray();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                EnsureRequiredScenes();
        }
    }
}
#endif
