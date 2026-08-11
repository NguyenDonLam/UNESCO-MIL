using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    /// <summary>Provides the built-in Scene 1 leaked-image storyline.</summary>
    internal static class SceneOneScenarioFactory
    {
        private const string CommunicationsRoomCue = "background.communications_room";
        private static readonly MajorSceneId SceneOneMajorId = new("S01");

        public static IReadOnlyList<NarrativeSceneDefinition> CreateSceneDefinitions() => new[]
        {
            Definition(CreateOpeningScene(), 1170),
            Definition(Response("S01_RESPONSE_01_A", "Vy",
                "Em hiểu cần phản hồi, nhưng đăng lại có nghĩa page chính thức đang đưa bức ảnh đến thêm hàng chục nghìn người.", "vy.uncomfortable",
                "Bài của ban tổ chức được chia sẻ rộng. Một số người lần đầu biết đến bức ảnh qua chính thông báo đang xác minh.", "S01_MERGE_01"), 1160),
            Definition(Response("S01_RESPONSE_01_B", "Cô Hương",
                "Được. Nhưng thông báo phải đủ rõ để công chúng không nghĩ mình đang né tránh.", "huong.tense",
                "Ban tổ chức phản hồi mà không khuếch đại thêm bức ảnh.", "S01_MERGE_01"), 1150),
            Definition(Response("S01_RESPONSE_01_C", "Cô Hương",
                "Nếu người ta chỉ bàn chuyện đời tư thì cô đồng ý. Nhưng họ đang dùng ảnh để đặt câu hỏi về quy trình của lễ hội.", "huong.tense",
                "Trong khoảng trống thông tin, các bài đăng khác bắt đầu thêm suy đoán về cách Vy được chọn.", "S01_MERGE_01"), 1140),
            Definition(CreateFirstMergeScene(), 1130),
            Definition(Response("S01_RESPONSE_02_A", "Vy",
                "Đã gặp nhau và được ưu ái là hai kết luận khác nhau. Nếu muốn biết em được chọn thế nào, mình nên kiểm tra quy trình tuyển chọn.", "vy.firm",
                "Trong bản nháp thông báo, bức ảnh bắt đầu bị dùng như bằng chứng về khả năng có xung đột lợi ích dù chưa được xác minh.", "S01_MERGE_02"), 1120),
            Definition(Response("S01_RESPONSE_02_B", "Vy",
                "Đúng. Nếu vấn đề là em được chọn như thế nào thì hãy kiểm tra bằng chứng về việc đó.", "vy.firm",
                "Đội truyền thông tách được sự thật đã biết khỏi suy luận chưa được chứng minh.", "S01_MERGE_02"), 1110),
            Definition(Response("S01_RESPONSE_02_C", "Cô Hương",
                "Quyền riêng tư là một vấn đề. Nhưng nếu ảnh đặt ra một câu hỏi thật về quy trình, mình vẫn phải kiểm tra câu hỏi đó bằng những nguồn phù hợp hơn.", "huong.tense",
                "Ban tổ chức bảo vệ quyền riêng tư nhưng chưa giải quyết được nghi vấn về tính minh bạch.", "S01_MERGE_02"), 1100),
            Definition(CreateSecondMergeScene(), 1090),
            Definition(Response("S01_RESPONSE_03_A", "Vy",
                "Nhưng cách đó cũng biến tất cả thông tin cá nhân của em thành tài liệu để công chúng phân tích.", "vy.firm",
                "Bài đính chính bác bỏ được cáo buộc tuyển chọn, nhưng ảnh và tin nhắn riêng của Vy tiếp tục bị phát tán, soi xét và kéo cả người thân của cô vào cuộc.", "S01_END"), 1080),
            Definition(Response("S01_RESPONSE_03_B", "Cô Hương",
                "Nếu công bố cả tiêu chí và cách ra quyết định, mọi người sẽ soi từng bước của quy trình. Nhưng mình có thể công bố đủ để kiểm chứng cáo buộc mà không mở toàn bộ đời tư của Vy.", "huong.concerned",
                "Phản hồi làm giảm suy đoán về Vy và giới hạn dữ liệu cá nhân bị công khai, đồng thời khiến ban tổ chức phải chịu trách nhiệm về tính minh bạch của quy trình.", "S01_END"), 1070),
            Definition(Response("S01_RESPONSE_03_C", "Vy",
                "Nhưng ảnh là thật. Nếu mình gọi tất cả là giả, họ chỉ cần đăng lại ảnh để nói ban tổ chức đang nói dối.", "vy.firm",
                "Cuộc tranh luận chuyển từ cáo buộc ban đầu sang câu hỏi ban tổ chức đang che giấu điều gì. Khủng hoảng tiếp tục tăng.", "S01_END"), 1060),
            Definition(CreateEndingScene(), 1050)
        };

        private static NarrativeScene CreateOpeningScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("Chưa đầy hai giờ trước khi lễ hội mở cửa.", CommunicationsRoomCue),
                Narration("Sau vụ việc của Mai An, phòng truyền thông tiếp tục nhận thêm hàng loạt thông báo."),
                Beat("Cô Hương", "Có một chuyện khác đang lan rất nhanh.", "huong.tense"),
                Beat("Cô Hương", "Có một bức ảnh chụp Vy, trưởng nhóm tình nguyện viên, đang ngồi riêng tại một quán cà phê với anh Quân, một thành viên ban tổ chức.", "huong.tense"),
                Beat("Cô Hương", "Bài đăng ám chỉ Vy được chọn vì quan hệ nội bộ và quy trình tuyển người không minh bạch. Nó đã có hơn 2.000 lượt chia sẻ.", "huong.tense"),
                Beat("Cô Hương", "Ban tài trợ vừa gửi ảnh chụp màn hình. Họ muốn biết cáo buộc có đúng không. Nếu mình im lặng, chuyện này sẽ trông như đang che giấu.", "huong.tense"),
                Beat("Vy", "Ảnh đó là ảnh của em. Nhưng em không biết ai chụp, và em chưa từng đồng ý cho nó được đăng.", "vy.uncomfortable"),
                Beat("Cô Hương", "Vậy ít nhất mình biết ảnh là thật. Cô nghĩ chúng ta nên phản hồi ngay trước khi câu chuyện đi xa hơn.", "huong.tense")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_1_1_A", "Mình nên đăng lại ảnh kèm thông báo rằng ban tổ chức đang xác minh. Như vậy mọi người biết chính xác mình đang nói về chuyện gì.", "S01_RESPONSE_01_A",
                    Rel("Vy", -2), Metric(MediaLiteracyMetric.Privacy, -2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s01_private_image_reposted")),
                Choice("CHOICE_1_1_B", "Không cần đăng lại ảnh. Mình có thể xác nhận rằng ban tổ chức đang kiểm tra cáo buộc về quy trình tuyển chọn.", "S01_RESPONSE_01_B",
                    Rel("Vy", 1), Metric(MediaLiteracyMetric.Privacy, 2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Crisis, -1), Flag("s01_process_review_announced")),
                Choice("CHOICE_1_1_C", "Đây là ảnh riêng tư của Vy. Mình không nên phản hồi gì cả.", "S01_RESPONSE_01_C",
                    Rel("Cô Hương", -1), Metric(MediaLiteracyMetric.Privacy, 1), Metric(MediaLiteracyMetric.Transparency, -2), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s01_no_initial_response"))
            };
            return Scene("S01_THE_LEAKED_IMAGE", beats, choices);
        }

        private static NarrativeScene CreateFirstMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Cô Hương", "Cô cần biết chính xác hôm đó Vy có gặp Quân không?", "huong.tense"),
                Beat("Vy", "Có. Em gặp anh Quân tại quán cà phê đó.", "vy.uncomfortable"),
                Beat("Cô Hương", "Vậy ít nhất phần đó đúng. Nếu công chúng hỏi, chúng ta phải giải thích mối quan hệ giữa hai người.", "huong.tense"),
                Beat("Vy", "Khoan đã. Em xác nhận bức ảnh là thật. Em chưa xác nhận chú thích của họ là thật.", "vy.firm"),
                Beat("Vy", "Một bức ảnh cho thấy em gặp anh Quân. Nó không tự chứng minh rằng em được chọn vì anh ấy.", "vy.firm")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_1_2_A", "Bức ảnh cho thấy Vy và một thành viên ban tổ chức có sự quen biết từ trước. Điều này đặt ra câu hỏi về tính độc lập của quyết định tuyển chọn.", "S01_RESPONSE_02_A",
                    Rel("Vy", -1), Metric(MediaLiteracyMetric.Evidence, -2), Metric(MediaLiteracyMetric.Privacy, -1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s01_image_treated_as_conflict_evidence")),
                Choice("CHOICE_1_2_B", "Ảnh chỉ xác nhận Vy và Quân đã gặp nhau. Lý do gặp và việc nó có liên quan đến tuyển chọn hay không thì chưa biết.", "S01_RESPONSE_02_B",
                    Rel("Vy", 1), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 1), Flag("s01_fact_separated_from_inference")),
                Choice("CHOICE_1_2_C", "Vì Vy không đồng ý cho đăng ảnh nên chúng ta không nên xem nó như bất kỳ loại bằng chứng nào.", "S01_RESPONSE_02_C",
                    Rel("Cô Hương", -1), Metric(MediaLiteracyMetric.Privacy, 1), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Transparency, -1), Flag("s01_image_entirely_dismissed"))
            };
            return Scene("S01_MERGE_01", beats, choices);
        }

        private static NarrativeScene CreateSecondMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("Bạn và cô Hương tìm thấy hồ sơ tuyển chọn nội bộ của lễ hội."),
                Narration("Hồ sơ cho thấy Vy nộp đơn trước buổi gặp trong ảnh và được phỏng vấn bởi một hội đồng ba người."),
                Narration("Quyết định chọn Vy được xác nhận hai ngày trước khi bức ảnh được chụp. Anh Quân không nằm trong hội đồng chấm ứng viên."),
                Beat("Cô Hương", "Vậy là rõ rồi. Mình có thể đăng tiến trình, biên bản tuyển chọn và cả bức ảnh để chứng minh cuộc gặp diễn ra sau quyết định.", "huong.relieved"),
                Beat("Vy", "Tại sao vẫn phải đăng ảnh? Nếu tiến trình và quy trình đã trả lời được cáo buộc thì công chúng cần ảnh của em để làm gì?", "vy.firm"),
                Beat("Cô Hương", "Cô muốn câu trả lời đủ thuyết phục. Nếu thiếu bằng chứng trực quan, người ta có thể nói chúng ta chỉ đang tự bảo vệ mình.", "huong.concerned"),
                Beat("Vy", "Minh bạch về quy trình không có nghĩa phải công khai thêm đời tư của em.", "vy.firm")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_1_3_A", "Nên công bố tất cả tài liệu, ảnh, tiến trình và các tin nhắn liên quan. Càng nhiều bằng chứng càng minh bạch.", "S01_RESPONSE_03_A",
                    Rel("Vy", -2), Metric(MediaLiteracyMetric.Transparency, 2), Metric(MediaLiteracyMetric.Privacy, -3), Metric(MediaLiteracyMetric.Empathy, -1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s01_all_private_material_published")),
                Choice("CHOICE_1_3_B", "Công bố tiến trình tuyển chọn, thành viên chấm điểm và thời điểm quyết định. Nói rõ dữ liệu cá nhân nào không được công khai và vì sao.", "S01_RESPONSE_03_B",
                    Rel("Vy", 2), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 2), Metric(MediaLiteracyMetric.Privacy, 2), Metric(MediaLiteracyMetric.Empathy, 1), Metric(MediaLiteracyMetric.Crisis, -2), Flag("s01_minimum_necessary_evidence_published")),
                Choice("CHOICE_1_3_C", "Ban tổ chức nên khẳng định mọi cáo buộc đang lan truyền hoàn toàn sai và yêu cầu cộng đồng ngừng chia sẻ.", "S01_RESPONSE_03_C",
                    Rel("Vy", 1), Metric(MediaLiteracyMetric.Transparency, -2), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s01_all_allegations_denied"))
            };
            return Scene("S01_MERGE_02", beats, choices);
        }

        private static NarrativeScene CreateEndingScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("CASE FILE 02 — TRUE, BUT NOT YOURS TO SHARE"),
                Narration("Một hình ảnh có thể hoàn toàn thật, nhưng điều đó không tự động tạo quyền để công khai hoặc tái sử dụng nó."),
                Narration("Khi xử lý thông tin cá nhân, hãy hỏi: Nó có thật không? Nó có thực sự chứng minh điều đang được tuyên bố không? Có cần công khai nó để giải quyết vấn đề không?"),
                Narration("Minh bạch không có nghĩa là công bố mọi thông tin đang có. Một phản hồi có trách nhiệm sử dụng lượng thông tin tối thiểu cần thiết để trả lời vấn đề công chúng có quyền quan tâm."),
                Narration("Câu hỏi cần nhớ: Mình có thể giải quyết vấn đề mà không phơi bày con người không?")
            };
            return Scene("S01_END", beats, new NarrativeChoice[0], new NarrativeSceneTransition(new MajorSceneId("S02")));
        }

        private static NarrativeScene Response(string id, string speaker, string dialogue, string spriteCue, string consequence, string destination)
        {
            var beats = new List<NarrativeBeat> { Beat(speaker, dialogue, spriteCue) };
            if (!string.IsNullOrWhiteSpace(consequence)) beats.Add(Narration(consequence));
            return Scene(id, beats, new NarrativeChoice[0], new NarrativeSceneTransition(null, new NarrativeSceneId(destination)));
        }

        private static NarrativeScene Scene(string id, IReadOnlyList<NarrativeBeat> beats, IReadOnlyList<NarrativeChoice> choices,
            params NarrativeSceneTransition[] transitions) =>
            new(new NarrativeSceneId(id), SceneOneMajorId, beats, choices, transitions);

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
