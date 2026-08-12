using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    /// <summary>Provides the built-in Scene 0 script until it is replaced by equivalent ScriptableObject assets.</summary>
    internal static class SceneZeroScenarioFactory
    {
        private const string CommunicationsRoomCue = "background.scene_0_misinformation";
        private static readonly MajorSceneId SceneZeroMajorId = new("S00");

        public static NarrativeSceneId OpeningSceneId => new("S00_01");

        public static IReadOnlyList<NarrativeSceneDefinition> CreateSceneDefinitions() => new[]
        {
            Definition(CreateOpeningScene(), 1300),
            Definition(Response("S00_RESPONSE_01_A", "Linh", "Được, tớ sẽ đăng ngay. Nếu có vấn đề thật thì mọi người cần biết càng sớm càng tốt.", "linh.worried",
                "Đoạn clip tiếp cận thêm hàng chục nghìn người qua trang của Linh.", "S00_MERGE_01"), 1290),
            Definition(Response("S00_RESPONSE_01_B", "Linh", "Được, tớ sẽ chờ. Nhưng mình cần kiểm tra nhanh vì bài đăng đang tiếp tục lan.", "linh.worried",
                "Linh tạm thời không chia sẻ clip.", "S00_MERGE_01"), 1280),
            Definition(Response("S00_RESPONSE_01_C", "Linh", "Ừ, cách này hợp lý hơn. Tớ vẫn có thể cảnh báo mọi người mà không làm đoạn clip lan thêm.", "linh.worried",
                "Linh đăng một thông báo trung lập, nhưng một số người tò mò bắt đầu tìm đoạn clip gốc.", "S00_MERGE_01"), 1270),
            Definition(CreateFirstMergeScene(), 1260),
            Definition(Response("S00_RESPONSE_02_A", "Minh", "Kết quả cao không có nghĩa công cụ chắc chắn đúng. Video bị nén hoặc đăng lại nhiều lần cũng có thể làm sai kết quả.", "minh.serious",
                null, "S00_MERGE_02"), 1250),
            Definition(Response("S00_RESPONSE_02_B", "Minh", "Chính xác. Nó có thể giúp mình đặt câu hỏi, nhưng không thể thay thế bằng chứng.", "minh.serious",
                null, "S00_MERGE_02"), 1240),
            Definition(Response("S00_RESPONSE_02_C", "Minh", "Không nên tin tuyệt đối vào công cụ. Nhưng bỏ qua hoàn toàn một manh mối cũng không phải là xác minh.", "minh.serious",
                null, "S00_MERGE_02"), 1230),
            Definition(CreateSecondMergeScene(), 1220),
            Definition(Response("S00_RESPONSE_03_A", "Minh", "Nhưng cô ấy thực sự đã nói những từ đó. Điều bị thao túng là ngữ cảnh và cách chúng được trình bày.", "minh.serious",
                "Công chúng tìm thấy bản gốc và cáo buộc ban tổ chức nói dối.", "S00_END"), 1210),
            Definition(Response("S00_RESPONSE_03_B", "Minh", "Cách giải thích này chính xác với bằng chứng chúng ta đang có.", "minh.serious",
                "Bản đầy đủ được công bố; lượng chia sẻ clip gây hiểu lầm bắt đầu giảm.", "S00_END"), 1200),
            Definition(Response("S00_RESPONSE_03_C", "Linh", "Nhưng hiện tại, bài đăng sai đang là nguồn thông tin duy nhất mà mọi người nhìn thấy.", "linh.worried",
                "Trong khoảng trống thông tin, các cáo buộc mới tiếp tục xuất hiện.", "S00_END"), 1190),
            Definition(CreateEndingScene(), 1180)
        };

        private static NarrativeScene CreateOpeningScene()
        {
            NarrativeSceneId id = OpeningSceneId;
            NarrativeBeat[] beats =
            {
                Narration("Chỉ còn ba giờ trước khi Lễ hội Sắc Việt Trẻ mở cửa.", CommunicationsRoomCue),
                Narration("Bạn là thành viên mới của đội truyền thông."),
                Beat("Linh", "Có chuyện rồi.", "linh.worried"),
                Beat("Linh", "Một đoạn clip về Mai An, ca sĩ đại diện lễ hội, đang lan khắp mạng xã hội.", "linh.worried"),
                Beat("Linh", "Trong clip, cô ấy nói: “Người ở An Phúc không hiểu nghệ thuật.”", "linh.worried"),
                Beat("Linh", "Clip chỉ dài tám giây, nhưng đã có hàng nghìn lượt chia sẻ. Mọi người đang yêu cầu ban tổ chức hủy tiết mục của cô ấy.", "linh.worried"),
                Beat("Linh", "Tớ có một trang gần 50.000 người theo dõi. Tớ nên xử lý đoạn clip này thế nào?", "linh.worried")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_0_1_A", "Cậu cứ đăng lại để cảnh báo mọi người. Clip đang lan nhanh nên không thể chờ lâu.", "S00_RESPONSE_01_A",
                    Rel("Linh", 1), Metric(MediaLiteracyMetric.Evidence, -2), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s00_clip_reposted")),
                Choice("CHOICE_0_1_B", "Đừng đăng lại vội. Hãy tìm bài đăng đầu tiên và bản phỏng vấn đầy đủ trước.", "S00_RESPONSE_01_B",
                    Rel("Linh", 1), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Crisis, -1), Flag("s00_verification_prioritized")),
                Choice("CHOICE_0_1_C", "Đừng chia sẻ lại clip. Cậu có thể đăng rằng thông tin đang được xác minh và nhắc mọi người chưa nên kết luận.", "S00_RESPONSE_01_C",
                    Rel("Linh", 1), Metric(MediaLiteracyMetric.Evidence, 1), Metric(MediaLiteracyMetric.Transparency, 2), Metric(MediaLiteracyMetric.Crisis, -1), Flag("s00_neutral_notice_posted"))
            };
            return Scene(id.Value, beats, choices);
        }

        private static NarrativeScene CreateFirstMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Minh", "Tớ vừa kiểm tra đoạn clip bằng một công cụ phát hiện nội dung AI.", "minh.serious"),
                Beat("Minh", "Kết quả cho thấy clip có 86% khả năng đã bị chỉnh sửa.", "minh.serious"),
                Beat("Linh", "Vậy là deepfake rồi đúng không? Chúng ta có thể công bố rằng Mai An chưa từng nói câu đó.", "linh.worried")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_0_2_A", "Đúng. 86% là đủ cao để kết luận đây là deepfake.", "S00_RESPONSE_02_A",
                    Rel("Minh", 1), Metric(MediaLiteracyMetric.Evidence, -2), Metric(MediaLiteracyMetric.Transparency, -1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s00_detector_treated_as_proof")),
                Choice("CHOICE_0_2_B", "Chưa thể kết luận. Công cụ chỉ cung cấp một dấu hiệu để điều tra tiếp.", "S00_RESPONSE_02_B",
                    Rel("Minh", 1), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 1), Flag("s00_detector_treated_as_clue")),
                Choice("CHOICE_0_2_C", "AI detector thường không đáng tin, nên kết quả này hoàn toàn vô dụng.", "S00_RESPONSE_02_C",
                    Rel("Minh", -2), Metric(MediaLiteracyMetric.Evidence, -1), Flag("s00_detector_dismissed"))
            };
            return Scene("S00_MERGE_01", beats, choices);
        }

        private static NarrativeScene CreateSecondMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Minh", "Tớ đã tìm được bản phỏng vấn đầy đủ.", "minh.serious"),
                Beat("Minh", "Trong đó, Mai An nói: “Không thể nói rằng người ở An Phúc không hiểu nghệ thuật. Vấn đề là chúng ta chưa tạo đủ cơ hội để họ tiếp cận.”", "minh.serious"),
                Beat("Linh", "Vậy từng từ trong clip đều là giọng thật của cô ấy. Nhưng người đăng đã cắt bỏ phần đầu câu.", "linh.surprised"),
                Beat("Minh", "Chưa hết. Biểu cảm trên gương mặt cô ấy cũng đã bị chỉnh sửa để trông khinh thường hơn.", "minh.serious"),
                Beat("Linh", "Vậy chúng ta nên nói với cộng đồng thế nào?", "linh.worried")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_0_3_A", "Clip hoàn toàn giả. Mai An không hề nói câu đó.", "S00_RESPONSE_03_A",
                    Rel("Minh", -1), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Transparency, -2), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s00_false_denial_published")),
                Choice("CHOICE_0_3_B", "Clip dùng lời nói thật nhưng đã cắt ngữ cảnh và chỉnh sửa hình ảnh để thay đổi ý nghĩa.", "S00_RESPONSE_03_B",
                    Rel("Linh", 1), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 2), Metric(MediaLiteracyMetric.Crisis, -3), Flag("s00_context_correction_published")),
                Choice("CHOICE_0_3_C", "Không cần phản hồi. Càng nói thì clip càng được chú ý.", "S00_RESPONSE_03_C",
                    Rel("Linh", -1), Metric(MediaLiteracyMetric.Privacy, 1), Metric(MediaLiteracyMetric.Transparency, -2), Metric(MediaLiteracyMetric.Crisis, 3), Flag("s00_no_response_published"))
            };
            return Scene("S00_MERGE_02", beats, choices);
        }

        private static NarrativeScene CreateEndingScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("CASE FILE 01 — TRUE CONTENT, FALSE IMPRESSION"),
                Narration("Bài học: Nội dung không cần được tạo hoàn toàn bằng AI để gây hiểu lầm. Một câu nói thật có thể trở thành misinformation khi bị cắt khỏi ngữ cảnh, ghép với caption định hướng hoặc chỉnh sửa hình ảnh."),
                Narration("Câu hỏi cần nhớ: Nội dung trước và sau đoạn trích là gì?"),
                Beat("Linh", "Có người vừa gửi cho tớ một bức ảnh nhạy cảm được cho là của Vy, trưởng nhóm tình nguyện viên.", "linh.worried"),
                Beat("Minh", "Và mọi người đang dùng nó để chứng minh rằng Vy có quan hệ với một thành viên ban tổ chức.", "minh.serious")
            };
            return Scene("S00_END", beats, new NarrativeChoice[0], new NarrativeSceneTransition(new MajorSceneId("S01")));
        }

        private static NarrativeScene Response(string id, string speaker, string dialogue, string spriteCue, string consequence, string destination)
        {
            var beats = new List<NarrativeBeat> { Beat(speaker, dialogue, spriteCue) };
            if (!string.IsNullOrWhiteSpace(consequence)) beats.Add(Narration(consequence));
            return Scene(id, beats, new NarrativeChoice[0], new NarrativeSceneTransition(null, new NarrativeSceneId(destination)));
        }

        private static NarrativeScene Scene(string id, IReadOnlyList<NarrativeBeat> beats, IReadOnlyList<NarrativeChoice> choices,
            params NarrativeSceneTransition[] transitions) =>
            new(new NarrativeSceneId(id), SceneZeroMajorId, beats, choices, transitions);

        private static NarrativeSceneDefinition Definition(NarrativeScene scene, int priority) =>
            new(scene, priority, new AlwaysSatisfiedSpecification());
        private static NarrativeBeat Beat(string speaker, string text, string spriteCue) =>
            new(new CharacterId(speaker), text, spriteCue);
        private static NarrativeBeat Narration(string text, string backgroundCue = null) =>
            new(null, text, null, backgroundCue);
        private static NarrativeChoice Choice(string id, string text, string destination, params IChoiceEffect[] effects) =>
            new(new ChoiceId(id), text, effects, new NarrativeSceneId(destination));
        private static RelationshipScoreChoiceEffect Rel(string character, int amount) =>
            new(new CharacterId(character), amount);
        private static MediaLiteracyScoreChoiceEffect Metric(MediaLiteracyMetric metric, int amount) => new(metric, amount);
        private static SetStoryFlagChoiceEffect Flag(string id) => new(new StoryFlagId(id));
    }
}
