using System.Collections.Generic;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    /// <summary>Provides the built-in Scene 2 secret-document storyline.</summary>
    internal static class SceneTwoScenarioFactory
    {
        private const string CommunicationsRoomCue = "background.scene_2_financial_leak";
        private static readonly MajorSceneId SceneTwoMajorId = new("S02");

        public static IReadOnlyList<NarrativeSceneDefinition> CreateSceneDefinitions() => new[]
        {
            Definition(CreateOpeningScene(), 1040),
            Definition(Branch("S02_RESPONSE_01_A", "S02_MERGE_01",
                Beat("Cô Hương", "Nhưng đội hậu cần đang không mở được hợp đồng và danh sách nhà cung cấp.", "huong.tense"),
                Narration("Rò rỉ từ thư mục dừng nhanh, nhưng hoạt động lễ hội bị gián đoạn. Một số khoản thanh toán và lượt check-in nhà cung cấp bị chậm.")), 1030),
            Definition(Branch("S02_RESPONSE_01_B", "S02_MERGE_01",
                Beat("Minh", "Em cần thời gian rà từng quyền truy cập. Chặn một đường dẫn chưa chắc có nghĩa mọi đường vào đều đã đóng.", "minh.serious"),
                Beat("Cô Hương", "Miễn là vận hành vẫn tiếp tục được.", "huong.concerned"),
                Narration("Đường dẫn đang được chia sẻ bị chặn. Một số nhân viên phải xác minh lại quyền, và Minh tiếp tục rà soát các đường truy cập khác.")), 1020),
            Definition(Branch("S02_RESPONSE_01_C", "S02_MERGE_01",
                Beat("Minh", "Giữ nguyên như hiện tại sẽ giúp mình nhìn rõ hoạt động đang xảy ra, nhưng em không thể bảo đảm không có thêm file bị lấy.", "minh.serious"),
                Beat("Cô Hương", "Ít nhất đội vận hành không bị gián đoạn và mình có thời gian trả lời nhà tài trợ.", "huong.tense"),
                Narration("Đội tiếp tục làm việc bình thường, nhưng trước khi quyền được thay đổi, một file nhà cung cấp khác bị tải xuống.")), 1010),
            Definition(CreateFirstMergeScene(), 1000),
            Definition(Branch("S02_RESPONSE_02_A", "S02_MERGE_02",
                Beat("Cô Hương", "Cách này ít nhất ngăn câu chuyện đi xa hơn.", "huong.relieved"),
                Beat("Minh", "Nhưng mình vẫn chưa kiểm tra việc chi tiêu thực tế. Nếu gọi cáo buộc là sai hoàn toàn, thông báo sẽ mạnh hơn bằng chứng hiện tại.", "minh.serious"),
                Narration("Bài đăng về 480 triệu mất đà trong ngắn hạn, nhưng ban tổ chức phải bảo vệ một tuyên bố chắc chắn khi việc kiểm tra vẫn đang diễn ra.")), 990),
            Definition(Branch("S02_RESPONSE_02_B", "S02_MERGE_02",
                Beat("Cô Hương", "Nhà tài trợ sẽ yêu cầu giải thích chi tiết hơn.", "huong.concerned"),
                Beat("Minh", "Nhưng mình đang xác nhận rõ phần nào đã biết và phần nào chưa.", "minh.serious"),
                Narration("Không có bài đăng mới quá nghiêm trọng, nhưng một số bình luận cho rằng ban tổ chức đang câu giờ. Áp lực chưa biến mất.")), 980),
            Definition(Branch("S02_RESPONSE_02_C", "S02_MERGE_02",
                Beat("Cô Hương", "Ít nhất ban tổ chức không phát biểu khi chưa chắc.", "huong.concerned"),
                Beat("Minh", "Nhưng chuyện file bị rò rỉ và chuyện 480 triệu được dùng thế nào là hai câu hỏi khác nhau.", "minh.serious"),
                Narration("Một trang đăng tiêu đề: Ban tổ chức chỉ nói về vụ rò rỉ, không trả lời tiền tài trợ đi đâu.")), 970),
            Definition(CreateSecondMergeScene(), 960),
            Definition(Branch("S02_RESPONSE_03_A", "S02_END",
                Beat("Minh", "Không sai. Nhưng mình đang bỏ qua một vấn đề chưa được làm rõ.", "minh.serious"),
                Narration("Scandal giảm nhanh và nhà tài trợ tạm yên tâm. Nếu hồ sơ 36 triệu xuất hiện sau đó, ban tổ chức sẽ phải giải thích vì sao nó không được nhắc tới.")), 950),
            Definition(Branch("S02_RESPONSE_03_B", "S02_END",
                Beat("Cô Hương", "Nhà tài trợ sẽ yêu cầu giải thích chi tiết hơn.", "huong.concerned"),
                Beat("Minh", "Nhưng mình đang xác nhận rõ điều đã biết, phần hồ sơ còn thiếu và giới hạn của kết luận hiện tại.", "minh.serious"),
                Narration("Phản hồi không làm tranh cãi biến mất ngay, nhưng công chúng nhận được một bản giải thích có thể kiểm chứng mà không biến nghi ngờ thành kết luận.")), 940),
            Definition(Branch("S02_RESPONSE_03_C", "S02_END",
                Beat("Minh", "Thiếu hóa đơn nên mình sẽ tiếp tục làm rõ, nhưng hiện tại không có bằng chứng về việc sử dụng sai tiền.", "minh.serious"),
                Beat("Cô Hương", "Nếu mình dùng cụm đó, mọi trang sẽ coi như ban tổ chức vừa thừa nhận scandal.", "huong.tense"),
                Narration("Tiêu đề Ban tổ chức thừa nhận đang điều tra sử dụng sai tiền tài trợ xuất hiện. Nhà tài trợ yêu cầu giải trình khẩn cấp.")), 930),
            Definition(CreateEndingScene(), 920)
        };

        private static NarrativeScene CreateOpeningScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("Chỉ còn hơn một giờ trước khi lễ hội bắt đầu. Sau hai cuộc khủng hoảng liên tiếp, ban tổ chức đang trong một bầu không khí căng thẳng.", CommunicationsRoomCue),
                Beat("Cô Hương", "Có một tài liệu nội bộ đang bị phát tán. Một file ngân sách của lễ hội đang được chia sẻ trên mạng.", "huong.tense"),
                Narration("Một dòng được đánh dấu: External Production & Advisory Services — 480.000.000 VND. Bên cạnh là tên Minh Phát Media."),
                Beat("Cô Hương", "Một bài viết cho rằng gần nửa tỷ tiền tài trợ đã bị chuyển vào một hạng mục mập mờ. Nhà tài trợ vừa gửi email yêu cầu giải thích.", "huong.tense"),
                Beat("Minh", "Tài liệu khớp với mẫu ngân sách nội bộ. Tên nhà cung cấp và khoản 480 triệu cũng có trong hệ thống.", "minh.serious"),
                Beat("Minh", "Có chuyện nghiêm trọng hơn. Link gốc dẫn tới một thư mục dùng chung từng được bật chế độ bất kỳ ai có link đều có thể xem, và link vẫn hoạt động.", "minh.serious"),
                Beat("Minh", "Thư mục chứa hợp đồng nhà cung cấp, bảng ngân sách và thông tin liên hệ nội bộ. Em muốn khóa toàn bộ quyền truy cập ngay.", "minh.serious")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_2_1_A", "Khóa toàn bộ thư mục và tài khoản liên quan ngay. Ngăn việc rò rỉ ngay lập tức.", "S02_RESPONSE_01_A",
                    Rel("Minh", 2), Metric(MediaLiteracyMetric.Privacy, 2), Metric(MediaLiteracyMetric.Crisis, -1), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Transparency, -1), Flag("s02_all_access_locked")),
                Choice("CHOICE_2_1_B", "Chặn link đang công khai nhưng giữ quyền cho các tài khoản nội bộ cần làm việc. Đồng thời rà xem còn link nào khác đang mở.", "S02_RESPONSE_01_B",
                    Rel("Minh", 1), Metric(MediaLiteracyMetric.Privacy, 1), Metric(MediaLiteracyMetric.Evidence, 2), Flag("s02_public_link_contained")),
                Choice("CHOICE_2_1_C", "Giữ thư mục hoạt động thêm một lúc. Minh theo dõi tài khoản truy cập, còn đội truyền thông xử lý nhà tài trợ và dư luận trước.", "S02_RESPONSE_01_C",
                    Rel("Minh", -2), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Privacy, -2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s02_folder_left_exposed"))
            };
            return Scene("S02_THE_SECRET_DOCUMENT", beats, choices);
        }

        private static NarrativeScene CreateFirstMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Cô Hương", "Giờ quay lại khoản 480 triệu. Hạng mục External Production & Support gồm sân khấu, livestream, nhân sự kỹ thuật và một số dịch vụ bên ngoài.", "huong.concerned"),
                Beat("Minh", "Em mới chỉ đối chiếu được bảng ngân sách tổng. Em chưa biết 480 triệu là ngân sách dự kiến hay số đã chi, đã thanh toán bao nhiêu và các hợp đồng có khớp không.", "minh.serious"),
                Beat("Minh", "Mình vẫn chưa biết toàn bộ số tiền đó đã được sử dụng như thế nào.", "minh.serious"),
                Beat("Cô Hương", "Nhà tài trợ đang chờ câu trả lời. Nếu mình nói chưa biết, họ sẽ nghĩ ban tổ chức không kiểm soát nổi ngân sách.", "huong.tense")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_2_2_A", "Nói rõ 480 triệu là ngân sách cho nhiều dịch vụ, không phải một khoản tiền thiếu minh bạch. Mình cần phản bác cáo buộc đang lan trước.", "S02_RESPONSE_02_A",
                    Rel("Minh", -1), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Crisis, -1), Flag("s02_claim_denied_before_review")),
                Choice("CHOICE_2_2_B", "Xác nhận 480 triệu là ngân sách cho nhiều hạng mục. Nói rõ ban tổ chức vẫn đang đối chiếu số đã chi và sẽ cập nhật khi kiểm tra xong.", "S02_RESPONSE_02_B",
                    Rel("Minh", 2), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 2), Flag("s02_known_and_unknown_disclosed")),
                Choice("CHOICE_2_2_C", "Chưa nói gì về con số. Chỉ xác nhận tài liệu nội bộ đã bị truy cập trái phép và ban tổ chức đang xử lý.", "S02_RESPONSE_02_C",
                    Rel("Minh", -1), Metric(MediaLiteracyMetric.Evidence, 1), Metric(MediaLiteracyMetric.Transparency, -2), Metric(MediaLiteracyMetric.Privacy, 1), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s02_only_breach_disclosed"))
            };
            return Scene("S02_MERGE_01", beats, choices);
        }

        private static NarrativeScene CreateSecondMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Minh", "Em và đội tài chính vừa đối chiếu thêm. 480 triệu không phải số đã thanh toán mà là mức ngân sách tối đa được phê duyệt cho toàn nhóm dịch vụ.", "minh.serious"),
                Beat("Minh", "Trong 312 triệu đã thanh toán có một khoản 36 triệu cho hỗ trợ kỹ thuật khẩn cấp sau sự cố sân khấu tuần trước.", "minh.serious"),
                Beat("Minh", "Đội kỹ thuật xác nhận công việc đã được thực hiện. Bộ phận tài chính có xác nhận thanh toán và biên bản công việc, nhưng hóa đơn chi tiết chưa đầy đủ.", "minh.serious"),
                Beat("Cô Hương", "Tức là khoản 480 triệu được chi hợp lý, chỉ là giấy tờ chưa hoàn tất. Nếu đưa 36 triệu vào thông báo, mọi người sẽ chỉ thấy một con số mới để nghi ngờ.", "huong.concerned"),
                Beat("Minh", "Nếu bỏ nó đi và tài liệu khác bị rò rỉ sau đó, họ sẽ hỏi tại sao ban tổ chức biết mà không nói.", "minh.serious"),
                Beat("Cô Hương", "Vậy câu trả lời nào vừa chính xác vừa không tự tạo thêm scandal?", "huong.tense")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_2_3_A", "Công bố 480 triệu là ngân sách tối đa và không có bằng chứng tiền tài trợ bị thất thoát. Không cần đưa khoản 36 triệu vào.", "S02_RESPONSE_03_A",
                    Rel("Minh", -1), Metric(MediaLiteracyMetric.Crisis, -2), Metric(MediaLiteracyMetric.Transparency, -1), Metric(MediaLiteracyMetric.Evidence, 1), Flag("s02_incomplete_issue_omitted")),
                Choice("CHOICE_2_3_B", "Công bố 480 triệu là ngân sách tối đa, 312 triệu đã thanh toán và phần lớn có đủ hồ sơ. Nói rõ khoản 36 triệu có dịch vụ thực tế nhưng hồ sơ đang được bổ sung.", "S02_RESPONSE_03_B",
                    Rel("Minh", 2), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 2), Flag("s02_responsible_financial_disclosure")),
                Choice("CHOICE_2_3_C", "Vì vẫn có 36 triệu chưa đủ hồ sơ, ban tổ chức nên nói đang điều tra thêm về một phần tiền tài trợ.", "S02_RESPONSE_03_C",
                    Rel("Minh", -2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Evidence, -2), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s02_missing_invoice_overstated"))
            };
            return Scene("S02_MERGE_02", beats, choices);
        }

        private static NarrativeScene CreateEndingScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("CASE FILE 03 — SUSPICION IS NOT PROOF"),
                Narration("Một thông báo chính xác có thể không dập scandal ngay, có thể khiến nhà tài trợ hỏi thêm và tạo thêm công việc."),
                Narration("Mục tiêu của truyền thông không chỉ là làm tranh cãi biến mất nhanh nhất, mà là nói đúng mức bằng chứng cho phép."),
                Narration("Câu hỏi cần nhớ: Chúng ta biết gì, còn thiếu gì, và có thể đưa ra khẳng định nào một cách có trách nhiệm?"),
                Beat("Minh", "Có thêm tin nhắn đang được chuyển tiếp. Một người nói nhiều khách ở khu ẩm thực đang bị ngộ độc và khuyên mọi người không tới lễ hội.", "minh.serious"),
                Beat("Cô Hương", "Có ai xác nhận chưa?", "huong.tense"),
                Beat("Minh", "Chưa. Nhưng nếu đây là nguy cơ sức khỏe thật, mình cũng không thể chờ quá lâu.", "minh.serious")
            };
            return Scene("S02_END", beats, new NarrativeChoice[0]);
        }

        private static NarrativeScene Branch(string id, string destination, params NarrativeBeat[] beats) =>
            Scene(id, beats, new NarrativeChoice[0], new NarrativeSceneTransition(null, new NarrativeSceneId(destination)));
        private static NarrativeScene Scene(string id, IReadOnlyList<NarrativeBeat> beats, IReadOnlyList<NarrativeChoice> choices,
            params NarrativeSceneTransition[] transitions) =>
            new(new NarrativeSceneId(id), SceneTwoMajorId, beats, choices, transitions);
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
