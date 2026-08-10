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
        public CompleteNarrativeSceneCommand(NarrativeSceneId sceneId) => SceneId = sceneId;
    }
    public readonly struct SelectNextNarrativeSceneCommand
    {
        public MajorSceneId MajorSceneId { get; }
        public NarrativeSceneId? DirectDestinationSceneId { get; }
        public SelectNextNarrativeSceneCommand(MajorSceneId majorSceneId, NarrativeSceneId? directDestinationSceneId = null)
        { MajorSceneId = majorSceneId; DirectDestinationSceneId = directDestinationSceneId; }
    }
    public readonly struct SaveGameCommand { }
    public readonly struct LoadGameCommand { }
    public readonly struct RestartGameCommand { }
}
