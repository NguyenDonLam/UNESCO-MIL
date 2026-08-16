using UnityEngine;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>One S-I-F-T step: its letter, title, description, and the accent colors used to render it.</summary>
    public readonly struct SIFTStep
    {
        public string Letter { get; }
        public string Title { get; }
        public string Description { get; }
        public Color AccentColor { get; }
        public Color IconFillColor { get; }
        public Color IconBorderColor { get; }

        public SIFTStep(string letter, string title, string description, Color accentColor, Color iconFillColor, Color iconBorderColor)
        {
            Letter = letter;
            Title = title;
            Description = description;
            AccentColor = accentColor;
            IconFillColor = iconFillColor;
            IconBorderColor = iconBorderColor;
        }
    }

    /// <summary>Canonical S-I-F-T step content shared by the opening information card and the persistent toolkit panel.</summary>
    internal static class SIFTStepData
    {
        public static readonly SIFTStep[] Steps =
        {
            new("S", "DỪNG LẠI", "Trước khi tin hay chia sẻ, hãy dừng lại và kiểm tra cảm xúc của bạn.",
                new Color(0.90f, 0.42f, 0.38f), new Color(0.62f, 0.20f, 0.18f), new Color(0.38f, 0.10f, 0.09f)),
            new("I", "KIỂM TRA NGUỒN", "Bạn có biết gì về nguồn thông tin này không? Nó có đáng tin không?",
                new Color(0.45f, 0.68f, 0.92f), new Color(0.16f, 0.35f, 0.55f), new Color(0.08f, 0.20f, 0.35f)),
            new("F", "TÌM NGUỒN TỐT HƠN", "Xem các nguồn uy tín khác có đưa tin tương tự không.",
                new Color(0.56f, 0.85f, 0.35f), new Color(0.24f, 0.48f, 0.16f), new Color(0.13f, 0.30f, 0.08f)),
            new("T", "TRUY NGUYÊN GỐC", "Tìm lại ngữ cảnh và nguồn gốc ban đầu của thông tin hoặc hình ảnh.",
                new Color(0.90f, 0.65f, 0.24f), new Color(0.62f, 0.40f, 0.10f), new Color(0.40f, 0.24f, 0.06f)),
        };
    }
}
