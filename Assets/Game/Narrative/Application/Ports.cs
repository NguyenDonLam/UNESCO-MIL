using System;
using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Application
{
    public interface INarrativeSceneRepository
    {
        NarrativeScene GetById(NarrativeSceneId id);
        IReadOnlyList<NarrativeSceneDefinition> GetCandidatesForMajorScene(MajorSceneId majorSceneId);
        NarrativeScene GetOpeningScene();
        IReadOnlyList<NarrativeSceneDefinition> GetAllDefinitions();
    }

    public interface INarrativePresentationGateway
    {
        void ShowSIFTInformationCard(Action onBeginCheck);
        void PresentScene(NarrativeScene scene, GameSession gameSession);
        void EndGame(NarrativeSceneId finalSceneId, GameSession gameSession);
    }

    public interface IUnitySceneLoadingAdapter
    {
        void LoadSceneAsync(string sceneName, Action onLoaded);
    }

    public sealed class GameSessionStore
    {
        public GameSession Current { get; private set; }
        public GameSession CreateNew() => Current = new GameSession();
    }
}
