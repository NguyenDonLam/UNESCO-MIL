using System;
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    public enum NarrativeValidationSeverity { Warning, Error }
    public sealed record NarrativeValidationResult(NarrativeValidationSeverity Severity, string Message);

    public sealed class NarrativeSceneValidator
    {
        public IReadOnlyList<NarrativeValidationResult> Validate(
            IReadOnlyList<NarrativeSceneDefinition> definitions,
            NarrativeSceneId openingSceneId,
            IReadOnlyCollection<CharacterId> knownCharacters,
            Func<string, bool> spriteCueExists = null,
            Func<string, bool> backgroundCueExists = null)
        {
            var results = new List<NarrativeValidationResult>();
            NarrativeScene[] scenes = definitions.Select(x => x.Scene).ToArray();
            foreach (IGrouping<NarrativeSceneId, NarrativeScene> duplicate in scenes.GroupBy(x => x.Id).Where(x => x.Count() > 1))
                Error(results, $"Duplicate scene ID: {duplicate.Key}.");
            if (!scenes.Any(x => x.Id == openingSceneId)) Error(results, $"Opening scene '{openingSceneId}' is missing.");

            HashSet<NarrativeSceneId> ids = scenes.Select(x => x.Id).ToHashSet();
            foreach (NarrativeScene scene in scenes)
            {
                if (scene.Beats.Count == 0) Error(results, $"Scene '{scene.Id}' has no dialogue beats.");
                foreach (NarrativeChoice choice in scene.Choices)
                {
                    if (!choice.Effects.OfType<RelationshipScoreChoiceEffect>().Any()) Error(results, $"Choice '{choice.Id}' has no relationship effect.");
                    if (choice.DirectDestinationSceneId is { } destination && !ids.Contains(destination)) Error(results, $"Choice '{choice.Id}' points to missing scene '{destination}'.");
                }
                foreach (NarrativeSceneTransition transition in scene.Transitions)
                    if (transition.DirectDestinationSceneId is { } destination && !ids.Contains(destination)) Error(results, $"Scene '{scene.Id}' points to missing scene '{destination}'.");
                if (scene.Choices.Count > 0 && scene.Transitions.Count == 0 && scene.Choices.All(x => !x.DirectDestinationSceneId.HasValue))
                    Error(results, $"Scene '{scene.Id}' has choices but no valid progression.");
                foreach (NarrativeBeat beat in scene.Beats)
                {
                    if (beat.SpeakerId is { } speaker && !knownCharacters.Contains(speaker)) Error(results, $"Scene '{scene.Id}' references unknown character '{speaker}'.");
                    if (!string.IsNullOrWhiteSpace(beat.CharacterSpriteCue) && spriteCueExists != null && !spriteCueExists(beat.CharacterSpriteCue)) Error(results, $"Scene '{scene.Id}' references missing sprite cue '{beat.CharacterSpriteCue}'.");
                    if (!string.IsNullOrWhiteSpace(beat.BackgroundCue) && backgroundCueExists != null && !backgroundCueExists(beat.BackgroundCue)) Error(results, $"Scene '{scene.Id}' references missing background cue '{beat.BackgroundCue}'.");
                }
            }

            foreach (IGrouping<MajorSceneId, NarrativeSceneDefinition> major in definitions.GroupBy(x => x.Scene.MajorSceneId))
            {
                if (major.Key.Value != "S1" && !major.Any(x => x.Specification is AlwaysSatisfiedSpecification)) Error(results, $"Major scene '{major.Key}' has no fallback candidate.");
                foreach (IGrouping<int, NarrativeSceneDefinition> priority in major.GroupBy(x => x.Priority).Where(x => x.Count() > 1))
                    Error(results, $"Major scene '{major.Key}' has ambiguous equal-priority candidates at priority {priority.Key}: {string.Join(", ", priority.Select(x => x.Scene.Id.Value))}.");
            }

            HashSet<MajorSceneId> reachableMajors = new();
            NarrativeScene opening = scenes.FirstOrDefault(x => x.Id == openingSceneId);
            if (opening != null) reachableMajors.Add(opening.MajorSceneId);
            bool changed;
            do
            {
                changed = false;
                foreach (NarrativeScene scene in scenes.Where(x => reachableMajors.Contains(x.MajorSceneId)))
                    foreach (MajorSceneId destination in scene.Transitions.Where(x => x.DestinationMajorSceneId.HasValue).Select(x => x.DestinationMajorSceneId.Value))
                        changed |= reachableMajors.Add(destination);
            } while (changed);
            foreach (NarrativeScene scene in scenes.Where(x => !reachableMajors.Contains(x.MajorSceneId))) Error(results, $"Scene '{scene.Id}' is unreachable from the opening scene.");
            return results;
        }

        private static void Error(ICollection<NarrativeValidationResult> results, string message) => results.Add(new NarrativeValidationResult(NarrativeValidationSeverity.Error, message));
    }
}
