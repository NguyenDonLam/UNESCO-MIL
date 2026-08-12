using System.Collections;
using System;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Narrative.Infrastructure;
using Bedrot.Narrative.Presentation;
using Bedrot.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Bedrot.Tests.PlayMode
{
    public sealed class NarrativePresentationPlayModeTests
    {
        [UnityTest]
        public IEnumerator GameSceneLoadsFromBuildSettings()
        {
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Game"));
        }

        [UnityTest]
        public IEnumerator GameSceneRunsLargeSemanticIdleAnimationsAndUsesCatalogueFacingDirections()
        {
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

            NarrativeScenePresenter scenePresenter = UnityEngine.Object.FindAnyObjectByType<NarrativeScenePresenter>();
            Assert.That(scenePresenter, Is.Not.Null);
            CharacterSpritePresenter characterPresenter = GetField<CharacterSpritePresenter>(scenePresenter, "characterSpritePresenter");
            Assert.That(characterPresenter, Is.Not.Null, "NarrativeScenePresenter must reference the presenter on CharacterLayer.");

            UnityCharacterSpriteAdapter leftAdapter = GetField<UnityCharacterSpriteAdapter>(characterPresenter, "leftAdapter");
            UnityCharacterSpriteAdapter rightAdapter = GetField<UnityCharacterSpriteAdapter>(characterPresenter, "rightAdapter");
            Assert.That(leftAdapter, Is.Not.Null);
            Assert.That(rightAdapter, Is.Not.Null);

            characterPresenter.PresentBeat(new NarrativeBeat(new CharacterId("Linh"), "Left", "linh.worried", SpriteSlot: CharacterSpriteSlot.Left));
            characterPresenter.PresentBeat(new NarrativeBeat(new CharacterId("Minh"), "Right", "minh.serious", SpriteSlot: CharacterSpriteSlot.Right));

            Animator leftAnimator = GetField<Animator>(leftAdapter, "targetAnimator");
            Animator rightAnimator = GetField<Animator>(rightAdapter, "targetAnimator");
            Assert.That(leftAnimator.runtimeAnimatorController.name, Is.EqualTo("linh-worried-idle"));
            Assert.That(rightAnimator.runtimeAnimatorController.name, Is.EqualTo("minh-serious-idle"));
            Assert.That(leftAnimator.enabled, Is.True);
            Assert.That(rightAnimator.enabled, Is.True);

            RectTransform leftTransform = GetField<Image>(leftAdapter, "targetImage").rectTransform;
            float initialVerticalPosition = leftTransform.anchoredPosition.y;
            Assert.That(Mathf.Abs(leftTransform.localScale.x), Is.GreaterThan(1.6f));
            Assert.That(leftTransform.localScale.y, Is.GreaterThan(1.6f));
            yield return new WaitForSeconds(0.9f);
            Assert.That(leftTransform.anchoredPosition.y, Is.GreaterThan(initialVerticalPosition),
                "The idle Animator should move the character through a stepped vertical bop.");
            Assert.That(leftTransform.localScale.y, Is.EqualTo(1.7141f).Within(0.001f),
                "The pixel bop must not stretch the sprite.");

            DialoguePresenter dialoguePresenter = GetField<DialoguePresenter>(scenePresenter, "dialoguePresenter");
            Assert.That(leftAdapter.transform.parent.GetSiblingIndex(),
                Is.LessThan(dialoguePresenter.transform.GetSiblingIndex()),
                "The character layer must render behind the dialogue panel.");

            characterPresenter.PresentBeat(new NarrativeBeat(new CharacterId("Vy"), "Right", "vy.firm", SpriteSlot: CharacterSpriteSlot.Right));
            yield return null;
            RectTransform vyTransform = GetField<Image>(rightAdapter, "targetImage").rectTransform;
            Assert.That(vyTransform.localScale.x, Is.LessThan(0f), "Vy should use her catalogue-authored facing direction.");

            characterPresenter.PresentBeat(new NarrativeBeat(new CharacterId("Huong"), "Left", "huong.concerned", SpriteSlot: CharacterSpriteSlot.Left));
            yield return null;
            Assert.That(leftAnimator.runtimeAnimatorController.name, Is.EqualTo("huong-concerned-idle"));
            Assert.That(leftTransform.localScale.x, Is.LessThan(0f),
                "Hương's opposite-facing source art should be mirrored by its catalogue entry.");
        }

        [UnityTest]
        public IEnumerator DialogueAdvancesAndChoiceButtonsAppear()
        {
            NarrativeScenePresenter presenter = CreatePresenterRig(out DialoguePresenter dialogue, out ChoicePresenter choices);
            var scene = new ScenarioFactory().CreateBuiltInSceneZeroDefinitions()[0].Scene;
            presenter.Bind(_ => { }, new GameEventPublisher()); presenter.PresentScene(scene);
            Assert.That(presenter.CurrentBeatIndex, Is.Zero);
            for (int i = 1; i < scene.Beats.Count; i++) presenter.AdvanceDialogue();
            presenter.AdvanceDialogue(); yield return null;
            Assert.That(choices.gameObject.activeSelf, Is.True);
            UnityEngine.Object.Destroy(presenter.transform.root.gameObject);
        }

        [UnityTest]
        public IEnumerator AdvanceCompletesTypewriterBeforeMovingToNextBeat()
        {
            NarrativeScenePresenter presenter = CreatePresenterRig(out DialoguePresenter dialogue, out _);
            var textObject = new GameObject("DialogueText", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(dialogue.transform);
            Text dialogueText = textObject.GetComponent<Text>();
            SetField(dialogue, "dialogueText", dialogueText);
            SetField(dialogue, "charactersPerSecond", 1f);
            var scene = new NarrativeScene(
                new NarrativeSceneId("TYPEWRITER_TEST"),
                new MajorSceneId("TYPEWRITER"),
                new[]
                {
                    new NarrativeBeat(new CharacterId("Linh"), "First line"),
                    new NarrativeBeat(new CharacterId("Minh"), "Second line")
                },
                Array.Empty<NarrativeChoice>(),
                Array.Empty<NarrativeSceneTransition>());

            presenter.Bind(_ => { }, new GameEventPublisher());
            presenter.PresentScene(scene);
            Assert.That(dialogue.IsRevealing, Is.True);

            presenter.AdvanceDialogue();
            Assert.That(presenter.CurrentBeatIndex, Is.Zero);
            Assert.That(dialogueText.text, Is.EqualTo("First line"));
            Assert.That(dialogue.IsRevealing, Is.False);

            presenter.AdvanceDialogue();
            Assert.That(presenter.CurrentBeatIndex, Is.EqualTo(1));
            Assert.That(dialogue.IsRevealing, Is.True);

            UnityEngine.Object.Destroy(presenter.transform.root.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DialogueBleepUsesSpeakerSignatureAndStopsWhenRevealCompletes()
        {
            var root = new GameObject("DialogueBleepTest", typeof(RectTransform));
            DialoguePresenter dialogue = root.AddComponent<DialoguePresenter>();
            var textObject = new GameObject("DialogueText", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(root.transform);
            SetField(dialogue, "dialogueText", textObject.GetComponent<Text>());
            SetField(dialogue, "charactersPerSecond", 1f);

            AudioClip narratorClip = AudioClip.Create("NarratorBleep", 4410, 1, 44100, false);
            AudioClip linhClip = AudioClip.Create("LinhBleep", 4410, 1, 44100, false);
            DialogueBleepCatalogueAsset catalogue = ScriptableObject.CreateInstance<DialogueBleepCatalogueAsset>();
            SetField(catalogue, "narratorClip", narratorClip);
            SetField(catalogue, "characterEntries", new List<DialogueBleepCueEntry>
            {
                new() { SpeakerId = "Linh", Clip = linhClip }
            });
            SetField(dialogue, "bleepCatalogue", catalogue);

            dialogue.PresentBeat(new NarrativeBeat(new CharacterId("Linh"), "A deliberately long line."));
            yield return null;

            AudioSource audioSource = GetField<AudioSource>(dialogue, "bleepAudioSource");
            Assert.That(audioSource.clip, Is.SameAs(linhClip));
            Assert.That(audioSource.loop, Is.True);
            Assert.That(audioSource.isPlaying, Is.True);

            Assert.That(dialogue.CompleteRevealImmediately(), Is.True);
            Assert.That(audioSource.isPlaying, Is.False);
            Assert.That(audioSource.clip, Is.Null);

            dialogue.PresentBeat(new NarrativeBeat(null, "Narrated text."));
            yield return null;
            Assert.That(audioSource.clip, Is.SameAs(narratorClip));

            UnityEngine.Object.Destroy(root);
            UnityEngine.Object.Destroy(catalogue);
            UnityEngine.Object.Destroy(linhClip);
            UnityEngine.Object.Destroy(narratorClip);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OpeningSceneAppearsInNarrativePresenter()
        {
            NarrativeScenePresenter presenter = CreatePresenterRig(out _, out _);
            presenter.Bind(_ => { }, new GameEventPublisher());
            presenter.PresentScene(new ScenarioFactory().CreateBuiltInSceneZeroDefinitions()[0].Scene);
            yield return null;
            Assert.That(presenter.CurrentScene.Id, Is.EqualTo(ScenarioFactory.BuiltInOpeningSceneId));
            UnityEngine.Object.Destroy(presenter.transform.root.gameObject);
        }

        [UnityTest]
        public IEnumerator SelectedChoiceChangesThePresentedScene()
        {
            var store = new GameSessionStore();
            var repository = new ScriptableObjectNarrativeSceneRepository(new ScenarioFactory().CreateBuiltInSceneZeroDefinitions(), ScenarioFactory.BuiltInOpeningSceneId);
            var events = new GameEventPublisher(); var presentation = new CapturePresentation();
            var next = new SelectNextNarrativeSceneCommandHandler(store, repository, new HighestPriorityNarrativeSceneSelectionStrategy(), presentation, events);
            var complete = new CompleteNarrativeSceneCommandHandler(store, repository, next, presentation, events);
            var choice = new SelectChoiceCommandHandler(store, repository, complete, events);
            new StartNewGameCommandHandler(store, repository, new ImmediateSceneLoadingAdapter(), presentation, events).Handle(new StartNewGameCommand());
            choice.Handle(new SelectChoiceCommand(new ChoiceId("CHOICE_0_1_B")));
            yield return null;
            Assert.That(presentation.Scene.Id.Value, Is.EqualTo("S00_RESPONSE_01_B"));
        }

        [UnityTest]
        public IEnumerator CharacterSpritePresenterShowsCueAndHidesForNarration()
        {
            var root = new GameObject("CharacterSpriteTest", typeof(RectTransform));
            var image = root.AddComponent<Image>();
            var adapter = root.AddComponent<UnityCharacterSpriteAdapter>();
            var presenter = root.AddComponent<CharacterSpritePresenter>();
            var catalogue = ScriptableObject.CreateInstance<CharacterSpriteCatalogueAsset>();
            var sprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
            SetField(catalogue, "entries", new System.Collections.Generic.List<CharacterAppearanceCueEntry> { new() { Cue = "linh.worried", Sprite = sprite } });
            SetField(adapter, "targetImage", image); SetField(adapter, "catalogue", catalogue); SetField(presenter, "leftAdapter", adapter);

            presenter.PresentBeat(new NarrativeBeat(new CharacterId("Linh"), "Có chuyện rồi.", "linh.worried"));
            Assert.That(image.enabled, Is.True); Assert.That(image.sprite, Is.SameAs(sprite));
            presenter.PresentBeat(new NarrativeBeat(null, "Narration"));
            Assert.That(image.enabled, Is.False);

            UnityEngine.Object.Destroy(root); UnityEngine.Object.Destroy(sprite.texture); UnityEngine.Object.Destroy(sprite); UnityEngine.Object.Destroy(catalogue);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BackgroundAdapterChangesExistingSpriteRendererFromSemanticCue()
        {
            var root = new GameObject("BackgroundAdapterTest");
            SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
            UnityBackgroundAdapter adapter = root.AddComponent<UnityBackgroundAdapter>();
            BackgroundCatalogueAsset catalogue = ScriptableObject.CreateInstance<BackgroundCatalogueAsset>();
            Sprite sprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
            SetField(catalogue, "entries", new List<SpriteCueEntry>
            {
                new() { Cue = "background.scene_test", Sprite = sprite }
            });
            SetField(adapter, "targetSpriteRenderer", renderer);
            SetField(adapter, "catalogue", catalogue);

            adapter.ShowBackground("background.scene_test");
            Assert.That(renderer.enabled, Is.True);
            Assert.That(renderer.sprite, Is.SameAs(sprite));

            adapter.HideBackground();
            Assert.That(renderer.enabled, Is.False);

            UnityEngine.Object.Destroy(root);
            UnityEngine.Object.Destroy(sprite.texture);
            UnityEngine.Object.Destroy(sprite);
            UnityEngine.Object.Destroy(catalogue);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterSpritePresentersRouteBeatsToLeftAndRightCharacters()
        {
            var root = new GameObject("TwoCharacterSpriteTest");
            var presenter = root.AddComponent<CharacterSpritePresenter>();
            UnityCharacterSpriteAdapter left = CreateCharacterAdapter(root.transform, "Left", "linh.worried", out Image leftImage);
            UnityCharacterSpriteAdapter right = CreateCharacterAdapter(root.transform, "Right", "minh.serious", out Image rightImage);
            SetField(presenter, "leftAdapter", left);
            SetField(presenter, "rightAdapter", right);

            presenter.PresentBeat(new NarrativeBeat(new CharacterId("Linh"), "Left speaks.", "linh.worried"));
            Assert.That(leftImage.enabled, Is.True);
            Assert.That(rightImage.enabled, Is.False);

            presenter.PresentBeat(new NarrativeBeat(new CharacterId("Minh"), "Right speaks.", "minh.serious"));
            Assert.That(leftImage.enabled, Is.True, "The left character should remain visible while the right character speaks.");
            Assert.That(rightImage.enabled, Is.True);

            UnityEngine.Object.Destroy(root);
            yield return null;
        }

        private static NarrativeScenePresenter CreatePresenterRig(out DialoguePresenter dialogue, out ChoicePresenter choices)
        {
            var root = new GameObject("NarrativeTestRig");
            var dialogueObject = new GameObject("Dialogue", typeof(RectTransform)); dialogueObject.transform.SetParent(root.transform);
            dialogue = dialogueObject.AddComponent<DialoguePresenter>();
            var choiceObject = new GameObject("Choices", typeof(RectTransform)); choiceObject.transform.SetParent(root.transform);
            choices = choiceObject.AddComponent<ChoicePresenter>();
            var button = new GameObject("ChoiceButton", typeof(RectTransform), typeof(Image), typeof(Button)); button.transform.SetParent(choiceObject.transform);
            var label = new GameObject("Label", typeof(RectTransform), typeof(Text)); label.transform.SetParent(button.transform);
            var presenter = root.AddComponent<NarrativeScenePresenter>();
            SetField(presenter, "dialoguePresenter", dialogue); SetField(presenter, "choicePresenter", choices);
            return presenter;
        }

        private static UnityCharacterSpriteAdapter CreateCharacterAdapter(Transform parent, string name, string cue, out Image image)
        {
            var characterObject = new GameObject(name, typeof(RectTransform));
            characterObject.transform.SetParent(parent);
            image = characterObject.AddComponent<Image>();
            var adapter = characterObject.AddComponent<UnityCharacterSpriteAdapter>();
            var catalogue = ScriptableObject.CreateInstance<CharacterSpriteCatalogueAsset>();
            var sprite = Sprite.Create(new Texture2D(2, 2), new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
            SetField(catalogue, "entries", new System.Collections.Generic.List<CharacterAppearanceCueEntry> { new() { Cue = cue, Sprite = sprite } });
            SetField(adapter, "targetImage", image);
            SetField(adapter, "catalogue", catalogue);
            adapter.HideSprite();
            return adapter;
        }
        private static void SetField(object target, string name, object value) => target.GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(target, value);
        private static T GetField<T>(object target, string name) =>
            (T)target.GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(target);

        private sealed class ImmediateSceneLoadingAdapter : IUnitySceneLoadingAdapter { public void LoadSceneAsync(string sceneName, Action onLoaded) => onLoaded(); }
        private sealed class CapturePresentation : INarrativePresentationGateway
        {
            public NarrativeScene Scene { get; private set; }
            public void PresentScene(NarrativeScene scene) => Scene = scene;
            public void EndGame(NarrativeSceneId finalSceneId, GameSession gameSession) { }
        }
    }
}
