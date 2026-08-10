using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    /// <summary>Isolates the code-authored first-playable content until equivalent ScriptableObject assets are created.</summary>
    public static class DemoScenarioFactory
    {
        public static IReadOnlyList<NarrativeSceneDefinition> CreateDemoScenarioDefinitions()
        {
            NarrativeScene opening = CreateOpeningScene();
            return new[]
            {
                new NarrativeSceneDefinition(opening, 0, new AlwaysSatisfiedSpecification()),
                Definition("S2_DANIEL_RECORDING", 100, new MinimumRelationshipScoreSpecification(new CharacterId("Daniel"), 2),
                    Beat("Daniel", "I sent you the full recording because you gave me time."),
                    Beat(null, "The longer video begins several minutes before the viral clip.")),
                Definition("S2_MINA_CONTEXT", 90, new MinimumRelationshipScoreSpecification(new CharacterId("Mina"), 2),
                    Beat("Mina", "I didn’t tell Sara everything. I was scared she would turn it into another headline.")),
                Definition("S2_SARA_ARTICLE", 80, new MinimumRelationshipScoreSpecification(new CharacterId("Sara"), 2),
                    Beat("Sara", "You made the call. Now help me decide how the article should frame it.")),
                Definition("S2_DEFAULT_VIRAL", 0, new AlwaysSatisfiedSpecification(),
                    Beat(null, "By morning, the clip has spread without any additional context."))
            };
        }

        public static NarrativeSceneId OpeningSceneId => new("S1_THE_CLIP");

        private static NarrativeScene CreateOpeningScene()
        {
            NarrativeSceneId sceneId = OpeningSceneId;
            var beats = new[]
            {
                Beat("Mina", "Have you seen this?"),
                Beat(null, "A short video shows Daniel standing over another student."),
                Beat("Daniel", "Maybe next time you’ll know when to keep your mouth shut."),
                Beat(null, "The clip ends. Its caption claims that Daniel threatened the student after being questioned about missing charity money."),
                Beat("Mina", "Sara wants to post it on the student news page tonight."),
                Beat("Mina", "I told her you were there earlier. She wants to know whether the caption is accurate."),
                Beat("Sara", "The clip is already spreading. If we wait, someone else will publish it first."),
                Beat("Sara", "Did Daniel threaten him?"),
                Beat(null, "Daniel sends you a private message."),
                Beat("Daniel", "The clip starts after he threatened Mina. Please don’t say anything until I send you the full recording."),
                Beat("Mina", "He did defend me, but I never heard anything about charity money."),
                Beat("Sara", "I need an answer now. What should I publish?")
            };
            var choices = new[]
            {
                Choice(sceneId, "S1_POST_WITHOUT_ACCUSATION", "Post the clip, but remove the charity accusation.",
                    new[] { Rel("Mina", 1), Rel("Daniel", -1), Rel("Sara", 2) }, "clip_published_without_accusation"),
                Choice(sceneId, "S1_WAIT_FOR_RECORDING", "Wait for Daniel’s full recording before publishing anything.",
                    new[] { Rel("Mina", -1), Rel("Daniel", 2), Rel("Sara", -1) }, "publication_delayed"),
                Choice(sceneId, "S1_PUBLISH_MINA_ACCOUNT", "Publish Mina’s account without using the video.",
                    new[] { Rel("Mina", 2), Rel("Daniel", -2), Rel("Sara", 1) }, "minas_account_published"),
                Choice(sceneId, "S1_NOT_ENOUGH_EVIDENCE", "Tell Sara that none of you currently have enough evidence.",
                    new[] { Rel("Mina", -1), Rel("Daniel", 1), Rel("Sara", -2) }, "refused_to_confirm")
            };
            return new NarrativeScene(sceneId, new MajorSceneId("S1"), beats, choices,
                new[] { new NarrativeSceneTransition(new MajorSceneId("S2")) });
        }

        private static NarrativeSceneDefinition Definition(string id, int priority, INarrativeSceneSpecification specification, params NarrativeBeat[] beats) =>
            new(new NarrativeScene(new NarrativeSceneId(id), new MajorSceneId("S2"), beats,
                new NarrativeChoice[0], new NarrativeSceneTransition[0]), priority, specification);
        private static NarrativeBeat Beat(string speaker, string text) =>
            new(string.IsNullOrEmpty(speaker) ? null : new CharacterId(speaker), text);
        private static RelationshipScoreChoiceEffect Rel(string character, int amount) => new(new CharacterId(character), amount);
        private static NarrativeChoice Choice(NarrativeSceneId sceneId, string idValue, string text,
            IReadOnlyList<RelationshipScoreChoiceEffect> relationships, string flag)
        {
            ChoiceId id = new(idValue);
            var effects = new List<IChoiceEffect>(relationships);
            effects.Add(new SetStoryFlagChoiceEffect(new StoryFlagId(flag)));
            effects.Add(new RecordChoiceChoiceEffect(id));
            effects.Add(new CompleteNarrativeSceneChoiceEffect(sceneId));
            return new NarrativeChoice(id, text, effects);
        }
    }
}
