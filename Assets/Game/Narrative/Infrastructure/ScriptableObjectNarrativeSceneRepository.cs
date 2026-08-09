using System;
using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Application;
using Bedrot.Narrative.Authoring;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEngine;

namespace Bedrot.Narrative.Infrastructure
{
    public sealed class ScriptableObjectNarrativeSceneRepository : INarrativeSceneRepository
    {
        private readonly IReadOnlyList<NarrativeSceneDefinition> _definitions;
        private readonly NarrativeSceneId _openingSceneId;

        public ScriptableObjectNarrativeSceneRepository(IEnumerable<NarrativeSceneCandidateAsset> candidates, NarrativeSceneAsset openingScene, ScenarioFactory factory)
            : this(candidates.Select(factory.CreateScenarioDefinition), new NarrativeSceneId(openingScene.SceneId)) { }

        public ScriptableObjectNarrativeSceneRepository(IEnumerable<NarrativeSceneDefinition> definitions, NarrativeSceneId openingSceneId)
        { _definitions = definitions.ToArray(); _openingSceneId = openingSceneId; }

        public NarrativeScene GetById(NarrativeSceneId id) => _definitions.Select(x => x.Scene).FirstOrDefault(x => x.Id == id)
            ?? throw new KeyNotFoundException($"Narrative scene '{id}' was not found.");
        public IReadOnlyList<NarrativeSceneDefinition> GetCandidatesForMajorScene(MajorSceneId majorSceneId) =>
            _definitions.Where(x => x.Scene.MajorSceneId == majorSceneId).ToArray();
        public NarrativeScene GetOpeningScene() => GetById(_openingSceneId);
        public IReadOnlyList<NarrativeSceneDefinition> GetAllDefinitions() => _definitions;
    }
}
