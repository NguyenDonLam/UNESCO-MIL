using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bedrot.Shared;

namespace Bedrot.Relationships.Domain
{
    public sealed class RelationshipState
    {
        private readonly Dictionary<CharacterId, int> _scores = new();

        public IReadOnlyDictionary<CharacterId, int> Scores =>
            new ReadOnlyDictionary<CharacterId, int>(_scores);

        public int GetScore(CharacterId characterId) =>
            _scores.TryGetValue(characterId, out int score) ? score : 0;

        public void ChangeScore(CharacterId characterId, int amount) =>
            _scores[characterId] = GetScore(characterId) + amount;

        public void SetScore(CharacterId characterId, int score) => _scores[characterId] = score;

        public void Clear() => _scores.Clear();
    }
}
