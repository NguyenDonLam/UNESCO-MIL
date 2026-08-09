using System.Collections.Generic;
using System.Linq;
using Bedrot.Shared;

namespace Bedrot.Narrative.Domain
{
    public interface INarrativeSceneSpecification
    {
        bool IsSatisfiedBy(GameSession gameSession);
    }

    public sealed class MinimumRelationshipScoreSpecification : INarrativeSceneSpecification
    {
        private readonly CharacterId _characterId; private readonly int _minimum;
        public MinimumRelationshipScoreSpecification(CharacterId characterId, int minimum) { _characterId = characterId; _minimum = minimum; }
        public bool IsSatisfiedBy(GameSession gameSession) => gameSession.Relationships.GetScore(_characterId) >= _minimum;
    }

    public sealed class MaximumRelationshipScoreSpecification : INarrativeSceneSpecification
    {
        private readonly CharacterId _characterId; private readonly int _maximum;
        public MaximumRelationshipScoreSpecification(CharacterId characterId, int maximum) { _characterId = characterId; _maximum = maximum; }
        public bool IsSatisfiedBy(GameSession gameSession) => gameSession.Relationships.GetScore(_characterId) <= _maximum;
    }

    public sealed class RelationshipScoreRangeSpecification : INarrativeSceneSpecification
    {
        private readonly MinimumRelationshipScoreSpecification _minimumSpecification;
        private readonly MaximumRelationshipScoreSpecification _maximumSpecification;
        public RelationshipScoreRangeSpecification(CharacterId characterId, int minimum, int maximum)
        {
            _minimumSpecification = new(characterId, minimum); _maximumSpecification = new(characterId, maximum);
        }
        public bool IsSatisfiedBy(GameSession gameSession) => _minimumSpecification.IsSatisfiedBy(gameSession) && _maximumSpecification.IsSatisfiedBy(gameSession);
    }

    public sealed class StoryFlagSetSpecification : INarrativeSceneSpecification
    {
        private readonly StoryFlagId _id;
        public StoryFlagSetSpecification(StoryFlagId id) => _id = id;
        public bool IsSatisfiedBy(GameSession gameSession) => gameSession.StoryFlags.Contains(_id);
    }

    public sealed class PreviousChoiceSelectedSpecification : INarrativeSceneSpecification
    {
        private readonly ChoiceId _id;
        public PreviousChoiceSelectedSpecification(ChoiceId id) => _id = id;
        public bool IsSatisfiedBy(GameSession gameSession) => gameSession.ChoiceHistory.Contains(_id);
    }

    public sealed class NarrativeSceneCompletedSpecification : INarrativeSceneSpecification
    {
        private readonly NarrativeSceneId _id;
        public NarrativeSceneCompletedSpecification(NarrativeSceneId id) => _id = id;
        public bool IsSatisfiedBy(GameSession gameSession) => gameSession.NarrativeProgress.IsCompleted(_id);
    }

    public sealed class AlwaysSatisfiedSpecification : INarrativeSceneSpecification
    {
        public bool IsSatisfiedBy(GameSession gameSession) => true;
    }

    public sealed class AllSceneConditionsCompositeSpecification : INarrativeSceneSpecification
    {
        private readonly IReadOnlyList<INarrativeSceneSpecification> _specifications;
        public AllSceneConditionsCompositeSpecification(IEnumerable<INarrativeSceneSpecification> specifications) => _specifications = specifications.ToArray();
        public bool IsSatisfiedBy(GameSession gameSession) => _specifications.All(x => x.IsSatisfiedBy(gameSession));
    }

    public sealed class AnySceneConditionsCompositeSpecification : INarrativeSceneSpecification
    {
        private readonly IReadOnlyList<INarrativeSceneSpecification> _specifications;
        public AnySceneConditionsCompositeSpecification(IEnumerable<INarrativeSceneSpecification> specifications) => _specifications = specifications.ToArray();
        public bool IsSatisfiedBy(GameSession gameSession) => _specifications.Any(x => x.IsSatisfiedBy(gameSession));
    }

    public sealed class NotSceneConditionCompositeSpecification : INarrativeSceneSpecification
    {
        private readonly INarrativeSceneSpecification _specification;
        public NotSceneConditionCompositeSpecification(INarrativeSceneSpecification specification) => _specification = specification;
        public bool IsSatisfiedBy(GameSession gameSession) => !_specification.IsSatisfiedBy(gameSession);
    }
}
