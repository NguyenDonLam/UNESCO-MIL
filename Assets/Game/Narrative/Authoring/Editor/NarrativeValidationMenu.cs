#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Bedrot.Narrative.Authoring.Editor
{
    public static class NarrativeValidationMenu
    {
        [MenuItem("Bedrot/Validate Narrative")]
        public static void ValidateNarrative()
        {
            IReadOnlyList<NarrativeSceneDefinition> definitions = new ScenarioFactory().CreateBuiltInSceneZeroDefinitions();
            var validator = new NarrativeSceneValidator();
            IReadOnlyList<NarrativeValidationResult> results = validator.Validate(definitions, ScenarioFactory.BuiltInOpeningSceneId,
                new[] { new CharacterId("Linh"), new CharacterId("Minh"), new CharacterId("Vy") });
            if (results.Count == 0) { Debug.Log($"Bedrot narrative validation passed ({definitions.Count} scene definitions)."); return; }
            foreach (NarrativeValidationResult result in results)
            {
                if (result.Severity == NarrativeValidationSeverity.Error) Debug.LogError($"Bedrot narrative: {result.Message}");
                else Debug.LogWarning($"Bedrot narrative: {result.Message}");
            }
            Debug.Log($"Bedrot narrative validation finished with {results.Count(x => x.Severity == NarrativeValidationSeverity.Error)} error(s).");
        }
    }

    public sealed class NarrativeBuildValidationProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;
        public void OnPreprocessBuild(BuildReport report)
        {
            IReadOnlyList<NarrativeSceneDefinition> definitions = new ScenarioFactory().CreateBuiltInSceneZeroDefinitions();
            var results = new NarrativeSceneValidator().Validate(definitions, ScenarioFactory.BuiltInOpeningSceneId,
                new[] { new CharacterId("Linh"), new CharacterId("Minh"), new CharacterId("Vy") });
            NarrativeValidationResult[] errors = results.Where(x => x.Severity == NarrativeValidationSeverity.Error).ToArray();
            if (errors.Length > 0)
                throw new BuildFailedException("Bedrot narrative validation failed:\n" + string.Join("\n", errors.Select(x => x.Message)));
        }
    }
}
#endif
