using System;
using System.Collections.Generic;
using System.Linq;

namespace Bedrot.Narrative.Domain
{
    public interface INarrativeSceneSelectionStrategy
    {
        NarrativeSceneDefinition SelectScene(IReadOnlyList<NarrativeSceneDefinition> candidates, GameSession gameSession);
    }

    public sealed class HighestPriorityNarrativeSceneSelectionStrategy : INarrativeSceneSelectionStrategy
    {
        public NarrativeSceneDefinition SelectScene(IReadOnlyList<NarrativeSceneDefinition> candidates, GameSession gameSession)
        {
            if (candidates == null) throw new ArgumentNullException(nameof(candidates));
            return candidates.Where(x => x.Specification.IsSatisfiedBy(gameSession))
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Scene.Id.Value, StringComparer.Ordinal)
                .FirstOrDefault()
                ?? throw new InvalidOperationException("No eligible narrative scene exists. Add an AlwaysSatisfiedSpecification fallback candidate.");
        }
    }
}
