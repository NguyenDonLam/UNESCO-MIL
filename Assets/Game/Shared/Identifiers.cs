using System;

namespace Bedrot.Shared
{
    public readonly struct CharacterId : IEquatable<CharacterId>
    {
        public string Value { get; }
        public CharacterId(string value) => Value = IdentifierGuard.Require(value, nameof(value));
        public bool Equals(CharacterId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is CharacterId other && Equals(other);
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);
        public override string ToString() => Value;
        public static bool operator ==(CharacterId left, CharacterId right) => left.Equals(right);
        public static bool operator !=(CharacterId left, CharacterId right) => !left.Equals(right);
    }
    public readonly struct NarrativeSceneId : IEquatable<NarrativeSceneId>
    {
        public string Value { get; }
        public NarrativeSceneId(string value) => Value = IdentifierGuard.Require(value, nameof(value));
        public bool Equals(NarrativeSceneId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is NarrativeSceneId other && Equals(other);
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);
        public override string ToString() => Value;
        public static bool operator ==(NarrativeSceneId left, NarrativeSceneId right) => left.Equals(right);
        public static bool operator !=(NarrativeSceneId left, NarrativeSceneId right) => !left.Equals(right);
    }
    public readonly struct MajorSceneId : IEquatable<MajorSceneId>
    {
        public string Value { get; }
        public MajorSceneId(string value) => Value = IdentifierGuard.Require(value, nameof(value));
        public bool Equals(MajorSceneId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is MajorSceneId other && Equals(other);
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);
        public override string ToString() => Value;
        public static bool operator ==(MajorSceneId left, MajorSceneId right) => left.Equals(right);
        public static bool operator !=(MajorSceneId left, MajorSceneId right) => !left.Equals(right);
    }
    public readonly struct ChoiceId : IEquatable<ChoiceId>
    {
        public string Value { get; }
        public ChoiceId(string value) => Value = IdentifierGuard.Require(value, nameof(value));
        public bool Equals(ChoiceId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is ChoiceId other && Equals(other);
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);
        public override string ToString() => Value;
        public static bool operator ==(ChoiceId left, ChoiceId right) => left.Equals(right);
        public static bool operator !=(ChoiceId left, ChoiceId right) => !left.Equals(right);
    }
    public readonly struct StoryFlagId : IEquatable<StoryFlagId>
    {
        public string Value { get; }
        public StoryFlagId(string value) => Value = IdentifierGuard.Require(value, nameof(value));
        public bool Equals(StoryFlagId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is StoryFlagId other && Equals(other);
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);
        public override string ToString() => Value;
        public static bool operator ==(StoryFlagId left, StoryFlagId right) => left.Equals(right);
        public static bool operator !=(StoryFlagId left, StoryFlagId right) => !left.Equals(right);
    }
    internal static class IdentifierGuard
    {
        public static string Require(string value, string name) => !string.IsNullOrWhiteSpace(value)
            ? value : throw new ArgumentException("An ID cannot be empty.", name);
    }
}
