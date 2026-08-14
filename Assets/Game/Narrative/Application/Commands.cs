using Bedrot.Shared;

namespace Bedrot.Narrative.Application
{
    public readonly struct StartNewGameCommand { }
    public readonly struct SelectChoiceCommand
    {
        public ChoiceId ChoiceId { get; }
        public SelectChoiceCommand(ChoiceId choiceId) => ChoiceId = choiceId;
    }
    public readonly struct CompleteNarrativeSceneCommand
    {
        public NarrativeSceneId SceneId { get; }
        public NarrativeSceneId? DirectDestinationSceneId { get; }
        public CompleteNarrativeSceneCommand(NarrativeSceneId sceneId, NarrativeSceneId? directDestinationSceneId = null)
        { SceneId = sceneId; DirectDestinationSceneId = directDestinationSceneId; }
    }
    public readonly struct SelectNextNarrativeSceneCommand
    {
        public MajorSceneId MajorSceneId { get; }
        public NarrativeSceneId? DirectDestinationSceneId { get; }
        public SelectNextNarrativeSceneCommand(MajorSceneId majorSceneId, NarrativeSceneId? directDestinationSceneId = null)
        { MajorSceneId = majorSceneId; DirectDestinationSceneId = directDestinationSceneId; }
    }
    public readonly struct RestartGameCommand { }
    public readonly struct SubmitCommunityContributionCommand
    {
        public string SituationType { get; }
        public string Description { get; }
        public bool ConsentToShare { get; }
        public SubmitCommunityContributionCommand(string situationType, string description, bool consentToShare)
        { SituationType = situationType; Description = description; ConsentToShare = consentToShare; }
    }
}
