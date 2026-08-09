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
        void PresentScene(NarrativeScene scene);
        void EndGame(NarrativeSceneId finalSceneId);
    }

    public interface IUnitySceneLoadingAdapter
    {
        void LoadSceneAsync(string sceneName, Action onLoaded);
    }

    public interface ISaveGameRepository
    {
        void Save(Save.GameSessionMemento memento);
        Save.GameSessionMemento Load();
        bool HasSave { get; }
        void Delete();
    }

    public sealed class GameSessionStore
    {
        public GameSession Current { get; private set; }
        public GameSession CreateNew() => Current = new GameSession();
        public void Replace(GameSession session) => Current = session ?? throw new ArgumentNullException(nameof(session));
    }
}
