using System.Collections.Generic;

namespace Bedrot.Narrative.Domain
{
    public enum MediaLiteracyMetric
    {
        Evidence,
        Empathy,
        Transparency,
        Privacy,
        Crisis
    }

    public sealed class MediaLiteracyState
    {
        private readonly Dictionary<MediaLiteracyMetric, int> _scores = new();
        public int GetScore(MediaLiteracyMetric metric) => _scores.TryGetValue(metric, out int score) ? score : 0;
        public void ChangeScore(MediaLiteracyMetric metric, int amount) => _scores[metric] = GetScore(metric) + amount;
    }
}
