namespace Bedrot.Narrative.Presentation
{
    /// <summary>A single metric or relationship value formatted for display in a consequence/case-file/ending screen.</summary>
    public sealed record VariableChangeSummary(string VariableName, int Delta)
    {
        public bool IsPositive => Delta >= 0;
    }
}
