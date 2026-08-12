#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Bedrot.Narrative.Authoring.Editor
{
    public static class WebGlBuildCommand
    {
        public static void Build()
        {
            RequiredBuildScenesEditorGuard.EnsureRequiredScenes();
            string outputPath = GetArgumentValue("-webGlOutputPath") ?? "Builds/WebGL";
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
                throw new InvalidOperationException("The WebGL build requires at least one enabled scene.");

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException(
                    $"WebGL build failed with {report.summary.totalErrors} error(s). See the Unity build log for details.");
        }

        private static string GetArgumentValue(string argumentName)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(arguments, argumentName);
            return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : null;
        }
    }
}
#endif
