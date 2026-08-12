using System;
using System.Collections.Generic;
using System.IO;
using Bedrot.Narrative.Presentation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Bedrot.Narrative.Authoring.Editor
{
    [InitializeOnLoad]
    public static class SemanticCharacterIdleAnimationGenerator
    {
        private const string CataloguePath = "Assets/Game/Narrative/Authoring/LeftCharacterSpriteCatalogue.asset";
        private const string OutputFolder = "Assets/Art/Sprites/Animation/SemanticIdle";

        private static readonly IReadOnlyDictionary<string, string> SpritePaths =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["linh.worried"] = "Assets/Art/Sprites/linh.worried.png",
                ["linh.surprised"] = "Assets/Art/Sprites/linh.surprised.png",
                ["minh.serious"] = "Assets/Art/Sprites/minh.serious.png",
                ["vy.firm"] = "Assets/Art/Sprites/vy.firm.png",
                ["vy.uncomfortable"] = "Assets/Art/Sprites/vy.uncomfortable.png",
                ["huong.tense"] = "Assets/Art/Sprites/huong.tense.png",
                ["huong.concerned"] = "Assets/Art/Sprites/huong.concerned.png",
                ["huong.relieved"] = "Assets/Art/Sprites/huong.relieved.png"
            };

        private static readonly ISet<string> HorizontallyFlippedCues =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "vy.firm",
                "vy.uncomfortable",
                "huong.tense",
                "huong.concerned",
                "huong.relieved"
            };

        static SemanticCharacterIdleAnimationGenerator()
        {
            EditorApplication.delayCall += GenerateMissingAssets;
        }

        [MenuItem("Bedrot/Generate Semantic Character Idle Animations")]
        public static void GenerateAll()
        {
            EnsureOutputFolder();
            CharacterSpriteCatalogueAsset catalogue =
                AssetDatabase.LoadAssetAtPath<CharacterSpriteCatalogueAsset>(CataloguePath);
            if (catalogue == null)
                throw new InvalidOperationException($"Character sprite catalogue was not found at '{CataloguePath}'.");

            var controllers = new Dictionary<string, AnimatorController>(StringComparer.Ordinal);
            var sprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, string> spriteDefinition in SpritePaths)
            {
                ConfigureSpriteImporter(spriteDefinition.Value);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spriteDefinition.Value);
                if (sprite == null)
                    throw new InvalidOperationException($"Sprite '{spriteDefinition.Value}' could not be imported.");

                sprites[spriteDefinition.Key] = sprite;
                controllers[spriteDefinition.Key] = CreateOrReplaceIdleController(spriteDefinition.Key, sprite);
            }

            UpdateCatalogue(catalogue, sprites, controllers);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated {controllers.Count} semantic character idle animations and updated the catalogue.");
        }

        private static void GenerateMissingAssets()
        {
            foreach (string cue in SpritePaths.Keys)
            {
                string assetName = cue.Replace('.', '-');
                string clipPath = $"{OutputFolder}/{assetName}-idle.anim";
                if (!File.Exists($"{OutputFolder}/{assetName}-idle.controller") ||
                    !File.Exists(clipPath) || HasTransformCurves(clipPath))
                {
                    GenerateAll();
                    return;
                }
            }
        }

        private static bool HasTransformCurves(string clipPath)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            return clip == null || AnimationUtility.GetCurveBindings(clip).Length > 0;
        }

        private static void ConfigureSpriteImporter(string spritePath)
        {
            AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
            if (AssetImporter.GetAtPath(spritePath) is not TextureImporter importer)
                throw new InvalidOperationException($"'{spritePath}' is not a texture asset.");

            bool changed = importer.textureType != TextureImporterType.Sprite ||
                           importer.spriteImportMode != SpriteImportMode.Single || importer.mipmapEnabled ||
                           importer.filterMode != FilterMode.Point;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point;
            if (changed) importer.SaveAndReimport();
        }

        private static AnimatorController CreateOrReplaceIdleController(string cue, Sprite sprite)
        {
            string assetName = cue.Replace('.', '-');
            string clipPath = $"{OutputFolder}/{assetName}-idle.anim";
            string controllerPath = $"{OutputFolder}/{assetName}-idle.controller";
            AssetDatabase.DeleteAsset(clipPath);
            AssetDatabase.DeleteAsset(controllerPath);

            var clip = new AnimationClip { name = $"{assetName}-idle", frameRate = 60f };
            SetLooping(clip);
            SetSpriteCurve(clip, sprite);
            AssetDatabase.CreateAsset(clip, clipPath);

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            AnimatorState state = controller.layers[0].stateMachine.AddState(clip.name);
            state.motion = clip;
            controller.layers[0].stateMachine.defaultState = state;
            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void SetSpriteCurve(AnimationClip clip, Sprite sprite)
        {
            var binding = new EditorCurveBinding
            {
                path = string.Empty,
                type = typeof(UnityEngine.UI.Image),
                propertyName = "m_Sprite"
            };
            AnimationUtility.SetObjectReferenceCurve(clip, binding,
                new[]
                {
                    new ObjectReferenceKeyframe { time = 0f, value = sprite },
                    new ObjectReferenceKeyframe { time = 3f, value = sprite }
                });
        }

        private static void SetLooping(AnimationClip clip)
        {
            SerializedObject serializedClip = new SerializedObject(clip);
            SerializedProperty settings = serializedClip.FindProperty("m_AnimationClipSettings");
            settings.FindPropertyRelative("m_LoopTime").boolValue = true;
            serializedClip.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void UpdateCatalogue(CharacterSpriteCatalogueAsset catalogue,
            IReadOnlyDictionary<string, Sprite> sprites,
            IReadOnlyDictionary<string, AnimatorController> controllers)
        {
            SerializedObject serializedCatalogue = new SerializedObject(catalogue);
            SerializedProperty entries = serializedCatalogue.FindProperty("entries");
            foreach (KeyValuePair<string, Sprite> spriteDefinition in sprites)
            {
                SerializedProperty entry = FindEntry(entries, spriteDefinition.Key);
                if (entry == null)
                {
                    entries.InsertArrayElementAtIndex(entries.arraySize);
                    entry = entries.GetArrayElementAtIndex(entries.arraySize - 1);
                    entry.FindPropertyRelative("Cue").stringValue = spriteDefinition.Key;
                }

                entry.FindPropertyRelative("Sprite").objectReferenceValue = spriteDefinition.Value;
                entry.FindPropertyRelative("AnimationController").objectReferenceValue =
                    controllers[spriteDefinition.Key];
                entry.FindPropertyRelative("AnimationStateName").stringValue = string.Empty;
                entry.FindPropertyRelative("FlipHorizontal").boolValue =
                    HorizontallyFlippedCues.Contains(spriteDefinition.Key);
            }

            serializedCatalogue.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalogue);
        }

        private static SerializedProperty FindEntry(SerializedProperty entries, string cue)
        {
            for (int index = 0; index < entries.arraySize; index++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(index);
                if (string.Equals(entry.FindPropertyRelative("Cue").stringValue, cue,
                        StringComparison.Ordinal))
                    return entry;
            }

            return null;
        }

        private static void EnsureOutputFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Art/Sprites/Animation"))
                throw new InvalidOperationException("The character animation folder is missing.");
            if (!AssetDatabase.IsValidFolder(OutputFolder))
                AssetDatabase.CreateFolder("Assets/Art/Sprites/Animation", "SemanticIdle");
        }
    }
}
