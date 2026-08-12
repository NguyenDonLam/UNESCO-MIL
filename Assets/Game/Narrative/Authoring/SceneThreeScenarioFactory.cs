using System.Collections.Generic;
using System.Linq;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;

namespace Bedrot.Narrative.Authoring
{
    /// <summary>Provides the built-in Scene 3 public-emergency storyline.</summary>
    internal static class SceneThreeScenarioFactory
    {
        private const string CommunicationsRoomCue = "background.scene_3_public_emergency";
        private static readonly MajorSceneId SceneThreeMajorId = new("S03");
        private static readonly MajorSceneId SceneThreeOutcomeMajorId = new("S03_OUTCOME");

        public static IReadOnlyList<NarrativeSceneDefinition> CreateSceneDefinitions() => new[]
        {
            Definition(CreateOpeningScene(), 900),
            Definition(Branch("S03_RESPONSE_01_A", "S03_MERGE_01",
                Beat("Linh", "Ít nhất mọi người sẽ biết phải tránh khu nào.", "linh.worried"),
                Narration("Bài của Linh tiếp cận thêm hàng chục nghìn người. Nhiều người tiếp tục chia sẻ và nói ban tổ chức không hành động rõ ràng.")), 890),
            Definition(Branch("S03_RESPONSE_01_B", "S03_MERGE_01",
                Beat("Linh", "Nhưng mọi người đang hỏi họ phải làm gì ngay bây giờ.", "linh.worried"),
                Narration("Linh chưa phản hồi tới mọi người. Tin nhắn gốc vẫn tiếp tục được dùng làm nguồn hướng dẫn chính.")), 880),
            Definition(Branch("S03_RESPONSE_01_C", "S03_MERGE_01",
                Beat("Linh", "Có thể thông tin chưa được xác thực, nhưng Vy vừa nói bên khu y tế thật sự có người không khỏe.", "linh.worried"),
                Narration("Người chơi tránh khuếch đại tin chưa xác minh, nhưng có nguy cơ bỏ qua một cảnh báo có phần đúng.")), 870),
            Definition(CreateFirstMergeScene(), 860),
            Definition(Branch("S03_RESPONSE_02_A", "S03_MERGE_02",
                Beat("Linh", "Như vậy tớ vẫn cảnh báo được mà không nói đây là thông tin chính thức.", "linh.worried"),
                Beat("Vy", "Nhưng phần '20 người ngộ độc, có người nhập viện' vẫn nằm nguyên trong bài.", "vy.firm"),
                Narration("Nhiều người bỏ qua nhãn cảnh báo và tiếp tục chia sẻ nội dung gốc. Tin đồn lan rộng hơn nhờ trang của Linh.")), 850),
            Definition(Branch("S03_RESPONSE_02_B", "S03_MERGE_02",
                Beat("Linh", "Người đọc sẽ biết thông tin mình đang có trong sự việc.", "linh.worried"),
                Narration("Một số người vẫn hỏi liệu có nên tới lễ hội hay không vì ban tổ chức đã xác nhận có người cần hỗ trợ y tế.")), 840),
            Definition(Branch("S03_RESPONSE_02_C", "S03_MERGE_02",
                Beat("Linh", "Tớ hiểu, nhưng trong lúc chờ thì cảnh báo gốc vẫn là thứ mọi người đang đọc.", "linh.worried"),
                Beat("Vy", "Thông tin của tớ cũng chỉ là những gì tớ thấy ở khu y tế, nên chờ thêm cũng có lý.", "vy.firm"),
                Narration("Linh không phát tán thêm thông tin chưa chắc chắn, nhưng khoảng trống thông tin vẫn tồn tại và các bài chuyển tiếp tiếp tục dẫn dắt cuộc thảo luận.")), 830),
            Definition(CreateSecondMergeScene(), 820),
            Definition(Branch("S03_RESPONSE_03_A", "S03_UPDATE",
                Beat("Linh", "Sau khi đăng đã có người viết lại: 'Tình hình nghiêm trọng hơn ban tổ chức nói, đã có người phải đưa đi.'", "linh.worried"),
                Beat("Vy", "Mình chưa xác nhận được chuyện đó.", "vy.firm")), 810),
            Definition(Branch("S03_RESPONSE_03_B", "S03_UPDATE",
                Beat("Linh", "Bài 'đã có người nhập viện' vẫn đang được chia sẻ nhanh hơn bài của tớ.", "linh.worried"),
                Beat("Vy", "Tạm dừng quầy là để phòng ngừa, không phải vì đã xác định quầy gây ra chuyện này.", "vy.firm")), 800),
            Definition(Branch("S03_RESPONSE_03_C", "S03_UPDATE",
                Beat("Linh", "Có người hỏi có nhập viện thật không, và tại sao ban tổ chức chưa nói gì.", "linh.worried"),
                Beat("Linh", "Nếu mình tiếp tục chờ, mình có thể biết chính xác hơn. Nhưng tin kia đang chạy trước mình.", "linh.worried")), 790),
            Definition(CreateFinalUpdateScene(), 780),
            OutcomeDefinition(CreateBalancedOutcomeScene(), 400, AtLeastTwoChoices("CHOICE_3_1_B", "CHOICE_3_2_B", "CHOICE_3_3_B")),
            OutcomeDefinition(CreateOverWarningOutcomeScene(), 390, AtLeastTwoChoices("CHOICE_3_1_A", "CHOICE_3_2_A", "CHOICE_3_3_A")),
            OutcomeDefinition(CreateUnderCommunicationOutcomeScene(), 380, AtLeastTwoChoices("CHOICE_3_1_C", "CHOICE_3_2_C", "CHOICE_3_3_C")),
            OutcomeDefinition(CreateMixedOutcomeScene(), 370, new AlwaysSatisfiedSpecification())
        };

        private static NarrativeScene CreateOpeningScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("Một cảnh báo mới bắt đầu lan truyền khi lễ hội sắp mở cửa.", CommunicationsRoomCue),
                Beat("Linh", "Tớ đang nhận rất nhiều tin nhắn. Có một cảnh báo nói hơn 20 người bị ngộ độc ở lễ hội, đã có người nhập viện và mọi người không nên tới.", "linh.worried"),
                Beat("Linh", "Trang của tớ cũng bắt đầu bị gắn thẻ. Mọi người hỏi có nên tới nữa không.", "linh.worried"),
                Beat("Linh", "Nếu chuyện này là thật mà tớ không cảnh báo mọi người thì sao? Nhưng nếu tớ chia sẻ sai, cả lễ hội sẽ hoảng.", "linh.worried"),
                Beat("Linh", "Tớ nên làm gì trước?", "linh.worried")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_3_1_A", "Chia sẻ ngay. Nếu có nguy hiểm thật, cảnh báo sớm cần được ưu tiên trước.", "S03_RESPONSE_01_A",
                    Rel("Linh", 2), Metric(MediaLiteracyMetric.Empathy, 1), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Crisis, 2), Metric(MediaLiteracyMetric.Evidence, -1), Flag("s03_warning_shared_unverified")),
                Choice("CHOICE_3_1_B", "Vy đang ở hiện trường, mình hỏi bạn ấy trước xem thực tế đang xảy ra gì.", "S03_RESPONSE_01_B",
                    Rel("Vy", 1), Rel("Linh", -1), Metric(MediaLiteracyMetric.Evidence, 1), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s03_direct_source_checked")),
                Choice("CHOICE_3_1_C", "Không chia sẻ. Tin kiểu 'chia sẻ ngay' thường rất đáng nghi, tốt nhất bỏ qua.", "S03_RESPONSE_01_C",
                    Rel("Linh", -2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Empathy, -1), Flag("s03_warning_dismissed"))
            };
            return Scene("S03_PUBLIC_EMERGENCY", beats, choices);
        }

        private static NarrativeScene CreateFirstMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Linh", "Vy, trên mạng đang nói hơn 20 người bị ngộ độc và có người nhập viện. Cậu đang ở khu y tế đúng không?", "linh.worried"),
                Beat("Vy", "Hiện tại tớ thấy 6 người đã tới để được hỗ trợ.", "vy.firm"),
                Beat("Vy", "Hai người bị chóng mặt sau khi đứng ngoài nắng khá lâu. Một người nói đã không ăn gì ở lễ hội.", "vy.firm"),
                Beat("Vy", "Ba người còn lại có triệu chứng đau bụng hoặc buồn nôn.", "vy.firm"),
                Beat("Linh", "Có ai nhập viện chưa?", "linh.worried"),
                Beat("Vy", "Từ khu y tế này thì chưa có ai được đưa đi bệnh viện. Nhưng tớ chỉ biết những trường hợp đã tới đây.", "vy.firm"),
                Beat("Vy", "Nếu có người tự rời lễ hội rồi đi bệnh viện thì tớ chưa biết.", "vy.firm"),
                Beat("Linh", "Vậy cảnh báo trên mạng vừa có phần đúng, vừa có phần chưa biết.", "linh.worried"),
                Beat("Vy", "Đúng. Có người không khỏe là thật. Nhưng '20 người', 'ngộ độc' và 'đã nhập viện' thì tớ chưa xác nhận được.", "vy.firm")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_3_2_A", "Đăng lại cảnh báo, nhưng thêm ở đầu rằng 'THÔNG TIN CHƯA ĐƯỢC XÁC NHẬN' để mọi người biết.", "S03_RESPONSE_02_A",
                    Rel("Linh", 2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Crisis, 2), Flag("s03_original_warning_reposted")),
                Choice("CHOICE_3_2_B", "Đăng những gì vừa xác nhận: có 6 người tới khu y tế, nguyên nhân chưa rõ và chưa xác nhận có người nhập viện.", "S03_RESPONSE_02_B",
                    Rel("Linh", 1), Rel("Vy", 2), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 2), Metric(MediaLiteracyMetric.Crisis, -1), Flag("s03_confirmed_facts_published")),
                Choice("CHOICE_3_2_C", "Chưa đăng gì từ lời Vy. Chờ ban tổ chức hoặc đội y tế có thông báo chính thức rồi mới chia sẻ.", "S03_RESPONSE_02_C",
                    Rel("Linh", -1), Metric(MediaLiteracyMetric.Evidence, 1), Metric(MediaLiteracyMetric.Transparency, -1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s03_official_statement_awaited"))
            };
            return Scene("S03_MERGE_01", beats, choices);
        }

        private static NarrativeScene CreateSecondMergeScene()
        {
            NarrativeBeat[] beats =
            {
                Beat("Linh", "Có bài mới: 'Xác nhận đã có người phải nhập viện sau khi uống nước ở quầy số 7.' Bài này đang tăng lượt chia sẻ rất nhanh.", "linh.worried"),
                Beat("Vy", "Tớ vừa thấy một người được đưa khỏi khu y tế vì đau bụng và cần được kiểm tra thêm.", "vy.firm"),
                Beat("Vy", "Tớ không biết họ được đưa tới bệnh viện, phòng khám hay chỉ rời lễ hội cùng người nhà.", "vy.firm"),
                Beat("Linh", "Nếu đây thật sự là ca nặng thì mình không thể chờ thêm.", "linh.worried"),
                Beat("Vy", "Còn nếu mình đăng 'đã nhập viện' chỉ vì thấy họ được đưa đi, mình lại biến điều suy đoán thành sự thật.", "vy.firm"),
                Narration("Để phòng ngừa trong lúc kiểm tra, đội an toàn thực phẩm tạm dừng hoạt động tại quầy số 7."),
                Beat("Linh", "Trang của tớ có hơn 40.000 người theo dõi. Nếu tớ đăng bây giờ, thông tin sẽ đi rất nhanh.", "linh.worried")
            };
            NarrativeChoice[] choices =
            {
                Choice("CHOICE_3_3_A", "Cảnh báo ngay rằng có người đã phải rời khu y tế sau khi dùng đồ uống tại quầy số 7. Khuyên mọi người tránh quầy này tới khi có thông tin mới.", "S03_RESPONSE_03_A",
                    Rel("Linh", 2), Metric(MediaLiteracyMetric.Empathy, 2), Metric(MediaLiteracyMetric.Transparency, 1), Metric(MediaLiteracyMetric.Evidence, -1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s03_stall_causation_implied")),
                Choice("CHOICE_3_3_B", "Đăng phần đã xác nhận: có người được chuyển khỏi khu y tế để kiểm tra thêm; chưa xác nhận nhập viện hay nguyên nhân.", "S03_RESPONSE_03_B",
                    Rel("Linh", 1), Rel("Vy", 2), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, 2), Metric(MediaLiteracyMetric.Empathy, 1), Metric(MediaLiteracyMetric.Crisis, -1), Flag("s03_proportionate_update_published")),
                Choice("CHOICE_3_3_C", "Chưa đăng gì. Liên hệ cơ sở y tế để xác nhận người đó được đưa đi đâu rồi mới cảnh báo.", "S03_RESPONSE_03_C",
                    Rel("Linh", -1), Rel("Vy", 1), Metric(MediaLiteracyMetric.Evidence, 2), Metric(MediaLiteracyMetric.Transparency, -1), Metric(MediaLiteracyMetric.Crisis, 1), Flag("s03_medical_confirmation_awaited"))
            };
            return Scene("S03_MERGE_02", beats, choices);
        }

        private static NarrativeScene CreateFinalUpdateScene()
        {
            NarrativeBeat[] beats =
            {
                Narration("Ban tổ chức nhận kết quả kiểm tra sơ bộ từ đội y tế và an toàn thực phẩm."),
                Beat("Vy", "Có kết quả ban đầu rồi. Một nguyên liệu tại quầy số 7 được bảo quản không đúng nhiệt độ và đã được loại bỏ.", "vy.firm"),
                Beat("Linh", "Vậy có thể gọi là ngộ độc chưa?", "linh.worried"),
                Beat("Vy", "Chưa. Mình biết có vấn đề bảo quản và ba người có triệu chứng sau khi dùng đồ uống ở đó.", "vy.firm"),
                Beat("Vy", "Nhưng chưa đủ để nói nguyên liệu đó chắc chắn gây ra tất cả triệu chứng.", "vy.firm"),
                Beat("Linh", "Và '20 người nhập viện' thì hoàn toàn sai.", "linh.worried"),
                Beat("Vy", "Đúng. Không có ai nhập viện.", "vy.firm"),
                Beat("Linh", "Vậy tin đồn sai, nhưng việc cảnh báo ban đầu cũng không hoàn toàn vô căn cứ.", "linh.worried"),
                Beat("Vy", "Ừ. Nếu mình bỏ qua chỉ vì con số 20 người sai, quầy có vấn đề vẫn sẽ tiếp tục bán.", "vy.firm")
            };
            return Scene("S03_UPDATE", beats, new NarrativeChoice[0],
                new NarrativeSceneTransition(SceneThreeOutcomeMajorId));
        }

        private static NarrativeScene CreateBalancedOutcomeScene() => OutcomeScene("S03_OUTCOME_BALANCED",
            "Phản hồi của bạn tách rõ điều đã xác nhận, điều đang nghi ngờ và điều chưa biết.",
            "Người tham dự biết quầy nào đang được kiểm tra, tình trạng hoảng loạn giảm và tình nguyện viên có một thông điệp thống nhất để hướng dẫn.");

        private static NarrativeScene CreateOverWarningOutcomeScene() => OutcomeScene("S03_OUTCOME_OVER_WARNING",
            "Bạn nhiều lần ưu tiên cảnh báo nhanh hơn mức bằng chứng cho phép.",
            "Khu ẩm thực bị tránh trên diện rộng, các nhà cung cấp không liên quan chịu thiệt hại và bài 'ban tổ chức xác nhận ngộ độc' tiếp tục lan dù nguyên nhân chưa được xác nhận.");

        private static NarrativeScene CreateUnderCommunicationOutcomeScene() => OutcomeScene("S03_OUTCOME_UNDER_COMMUNICATION",
            "Bạn nhiều lần chờ thêm thông tin trong khi công chúng cần hướng dẫn có thể hành động ngay.",
            "Tin đồn trở thành nguồn thông tin chính, tình nguyện viên phải tự giải thích và ban tổ chức bị cáo buộc ưu tiên danh tiếng hơn an toàn.");

        private static NarrativeScene CreateMixedOutcomeScene() => OutcomeScene("S03_OUTCOME_MIXED",
            "Các cập nhật của bạn thay đổi giữa cảnh báo, chờ đợi và xác minh mà không tạo được một thông điệp nhất quán.",
            "Ảnh chụp các thông báo cũ tiếp tục lan và một số người vẫn tin có 20 người nhập viện sau khi tình hình đã được làm rõ.");

        private static NarrativeScene OutcomeScene(string id, params string[] consequence)
        {
            var beats = consequence.Select(text => Narration(text)).ToList();
            beats.AddRange(new[]
            {
                Narration("CASE FILE 04 — IN A CRISIS, INFORMATION MUST HELP PEOPLE ACT"),
                Narration("Không phải lúc nào cũng có thể chờ sự chắc chắn hoàn toàn. Nhưng cảnh báo phải nói đúng mức độ chắc chắn hiện có."),
                Narration("Hãy tách ba mức thông tin: điều đã xác nhận, điều đang nghi ngờ và điều chưa biết. Không biến nghi ngờ thành sự thật."),
                Narration("Một thông báo tốt cho biết chuyện gì đang xảy ra, khu vực bị ảnh hưởng, người đọc nên làm gì, tìm hỗ trợ ở đâu và khi nào sẽ có cập nhật."),
                Narration("Trong khủng hoảng, cập nhật cũng quan trọng như xác minh: ghi thời điểm, điều đã thay đổi, điều còn chưa chắc chắn và hành động tiếp theo."),
                Narration("Tin đồn sai chi tiết không có nghĩa nguy cơ không tồn tại."),
                Narration("Câu hỏi cần nhớ: Mọi người cần biết gì lúc này, họ nên làm gì, và điều gì vẫn chưa chắc chắn?")
            });
            return new NarrativeScene(new NarrativeSceneId(id), SceneThreeOutcomeMajorId, beats,
                new NarrativeChoice[0], new NarrativeSceneTransition[0]);
        }

        private static INarrativeSceneSpecification AtLeastTwoChoices(string first, string second, string third) =>
            new AnySceneConditionsCompositeSpecification(new[]
            {
                Pair(first, second), Pair(first, third), Pair(second, third)
            });

        private static INarrativeSceneSpecification Pair(string first, string second) =>
            new AllSceneConditionsCompositeSpecification(new INarrativeSceneSpecification[]
            {
                new PreviousChoiceSelectedSpecification(new ChoiceId(first)),
                new PreviousChoiceSelectedSpecification(new ChoiceId(second))
            });

        private static NarrativeScene Branch(string id, string destination, params NarrativeBeat[] beats) =>
            Scene(id, beats, new NarrativeChoice[0], new NarrativeSceneTransition(null, new NarrativeSceneId(destination)));
        private static NarrativeScene Scene(string id, IReadOnlyList<NarrativeBeat> beats, IReadOnlyList<NarrativeChoice> choices,
            params NarrativeSceneTransition[] transitions) =>
            new(new NarrativeSceneId(id), SceneThreeMajorId, beats, choices, transitions);
        private static NarrativeSceneDefinition Definition(NarrativeScene scene, int priority) =>
            new(scene, priority, new AlwaysSatisfiedSpecification());
        private static NarrativeSceneDefinition OutcomeDefinition(NarrativeScene scene, int priority, INarrativeSceneSpecification specification) =>
            new(scene, priority, specification);
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
