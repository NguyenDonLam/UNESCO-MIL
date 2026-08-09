using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEngine;

namespace Bedrot.Narrative.Authoring
{
    public abstract class NarrativeSceneSpecificationAsset : ScriptableObject
    {
        public abstract INarrativeSceneSpecification CreateSpecification();
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Specifications/Minimum Relationship")]
    public sealed class MinimumRelationshipScoreSpecificationAsset : NarrativeSceneSpecificationAsset
    {
        public string CharacterId; public int MinimumScore;
        public override INarrativeSceneSpecification CreateSpecification() => new MinimumRelationshipScoreSpecification(new CharacterId(CharacterId), MinimumScore);
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Specifications/Story Flag Set")]
    public sealed class StoryFlagSetSpecificationAsset : NarrativeSceneSpecificationAsset
    {
        public string StoryFlagId;
        public override INarrativeSceneSpecification CreateSpecification() => new StoryFlagSetSpecification(new StoryFlagId(StoryFlagId));
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Specifications/Previous Choice")]
    public sealed class PreviousChoiceSelectedSpecificationAsset : NarrativeSceneSpecificationAsset
    {
        public string ChoiceId;
        public override INarrativeSceneSpecification CreateSpecification() => new PreviousChoiceSelectedSpecification(new ChoiceId(ChoiceId));
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Specifications/Always Satisfied")]
    public sealed class AlwaysSatisfiedSpecificationAsset : NarrativeSceneSpecificationAsset
    {
        public override INarrativeSceneSpecification CreateSpecification() => new AlwaysSatisfiedSpecification();
    }

    [CreateAssetMenu(menuName = "Bedrot/Narrative/Specifications/All Conditions")]
    public sealed class AllSceneConditionsCompositeSpecificationAsset : NarrativeSceneSpecificationAsset
    {
        public List<NarrativeSceneSpecificationAsset> Conditions = new();
        public override INarrativeSceneSpecification CreateSpecification() =>
            new AllSceneConditionsCompositeSpecification(Conditions.Where(x => x != null).Select(x => x.CreateSpecification()));
    }
}
