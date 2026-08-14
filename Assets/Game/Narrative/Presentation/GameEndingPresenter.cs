using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bedrot.Narrative.Domain;
using Bedrot.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    /// <summary>Presents the terminal screen after the final narrative scene, with a final variable snapshot and next-step choices.</summary>
    public sealed class GameEndingPresenter : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Text _messageText;
        private Transform _snapshotContainer;
        private GameObject _journeyPanel;
        private Text _journeyText;
        private bool _journeyExpanded;
        private Action _onPlayAgain;
        private Action _onShareExperience;
        private GameSession _lastSession;

        public static GameEndingPresenter Create(Transform parent)
        {
            var root = new GameObject("GameEndingView", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(GameEndingPresenter));
            if (parent != null) root.transform.SetParent(parent, false);

            RectTransform rootRect = root.GetComponent<RectTransform>();
            Stretch(rootRect);

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var background = new GameObject("BlackBackground", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(root.transform, false);
            Stretch(background.GetComponent<RectTransform>());
            background.GetComponent<Image>().color = Color.black;

            var message = new GameObject("EndingMessage", typeof(RectTransform), typeof(Text));
            message.transform.SetParent(root.transform, false);
            RectTransform messageRect = message.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0f, 0.42f);
            messageRect.anchorMax = new Vector2(1f, 1f);
            messageRect.offsetMin = new Vector2(60f, 0f);
            messageRect.offsetMax = new Vector2(-60f, -40f);
            Text text = message.GetComponent<Text>();
            text.text = string.Empty;
            text.alignment = TextAnchor.UpperCenter;
            text.color = Color.white;
            text.fontSize = 40;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 40;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var snapshot = new GameObject("FinalVariables", typeof(RectTransform));
            snapshot.transform.SetParent(root.transform, false);
            RectTransform snapshotRect = (RectTransform)snapshot.transform;
            snapshotRect.anchorMin = new Vector2(0.15f, 0.22f);
            snapshotRect.anchorMax = new Vector2(0.85f, 0.42f);
            snapshotRect.offsetMin = snapshotRect.offsetMax = Vector2.zero;
            var snapshotLayout = snapshot.AddComponent<GridLayoutGroup>();
            snapshotLayout.cellSize = new Vector2(280f, 36f);
            snapshotLayout.spacing = new Vector2(16f, 4f);
            snapshotLayout.childAlignment = TextAnchor.UpperCenter;

            var journeyPanel = new GameObject("JourneyPanel", typeof(RectTransform), typeof(Image));
            journeyPanel.transform.SetParent(root.transform, false);
            RectTransform journeyRect = (RectTransform)journeyPanel.transform;
            journeyRect.anchorMin = new Vector2(0.2f, 0.22f);
            journeyRect.anchorMax = new Vector2(0.8f, 0.9f);
            journeyRect.offsetMin = journeyRect.offsetMax = Vector2.zero;
            journeyPanel.GetComponent<Image>().color = new Color(0.05f, 0.06f, 0.08f, 0.97f);
            var journeyText = new GameObject("JourneyText", typeof(RectTransform), typeof(Text));
            journeyText.transform.SetParent(journeyPanel.transform, false);
            RectTransform journeyTextRect = journeyText.GetComponent<RectTransform>();
            journeyTextRect.anchorMin = Vector2.zero; journeyTextRect.anchorMax = Vector2.one;
            journeyTextRect.offsetMin = new Vector2(30f, 30f); journeyTextRect.offsetMax = new Vector2(-30f, -30f);
            Text journeyTextComponent = journeyText.GetComponent<Text>();
            journeyTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            journeyTextComponent.fontSize = 22;
            journeyTextComponent.color = Color.white;
            journeyTextComponent.alignment = TextAnchor.UpperLeft;
            journeyPanel.SetActive(false);

            var buttonRow = new GameObject("Buttons", typeof(RectTransform));
            buttonRow.transform.SetParent(root.transform, false);
            RectTransform buttonRowRect = (RectTransform)buttonRow.transform;
            buttonRowRect.anchorMin = new Vector2(0.5f, 0f);
            buttonRowRect.anchorMax = new Vector2(0.5f, 0f);
            buttonRowRect.pivot = new Vector2(0.5f, 0f);
            buttonRowRect.sizeDelta = new Vector2(1000f, 80f);
            buttonRowRect.anchoredPosition = new Vector2(0f, 50f);
            var buttonLayout = buttonRow.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 24f;
            buttonLayout.childControlWidth = false;
            buttonLayout.childControlHeight = false;
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;

            Button playAgainButton = RuntimeUIFactory.CreateButton(buttonRow.transform, "PlayAgainButton", "Chơi lại", new Color(0.2f, 0.55f, 0.3f), 26);
            playAgainButton.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 68f);
            Button reviewJourneyButton = RuntimeUIFactory.CreateButton(buttonRow.transform, "ReviewJourneyButton", "Xem lại hành trình", new Color(0.3f, 0.3f, 0.33f), 26);
            reviewJourneyButton.GetComponent<RectTransform>().sizeDelta = new Vector2(320f, 68f);
            Button shareButton = RuntimeUIFactory.CreateButton(buttonRow.transform, "ShareExperienceButton", "Chia sẻ trải nghiệm", new Color(0.2f, 0.4f, 0.65f), 26);
            shareButton.GetComponent<RectTransform>().sizeDelta = new Vector2(320f, 68f);

            GameEndingPresenter presenter = root.GetComponent<GameEndingPresenter>();
            presenter._canvasGroup = root.GetComponent<CanvasGroup>();
            presenter._messageText = text;
            presenter._snapshotContainer = snapshot.transform;
            presenter._journeyPanel = journeyPanel;
            presenter._journeyText = journeyTextComponent;
            playAgainButton.onClick.AddListener(presenter.OnPlayAgainClicked);
            reviewJourneyButton.onClick.AddListener(presenter.OnReviewJourneyClicked);
            shareButton.onClick.AddListener(presenter.OnShareExperienceClicked);
            presenter.Hide();
            return presenter;
        }

        public void Bind(Action onPlayAgain, Action onShareExperience)
        {
            _onPlayAgain = onPlayAgain;
            _onShareExperience = onShareExperience;
        }

        public void ShowResults(GameSession gameSession)
        {
            if (gameSession == null) throw new ArgumentNullException(nameof(gameSession));
            _lastSession = gameSession;
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_messageText != null) _messageText.text = BuildResultsText(gameSession);
            _journeyExpanded = false;
            if (_journeyPanel != null) _journeyPanel.SetActive(false);
            PopulateFinalSnapshot(gameSession);
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public static string BuildResultsText(GameSession gameSession)
        {
            if (gameSession == null) throw new ArgumentNullException(nameof(gameSession));

            var text = new StringBuilder("NHẬN XÉT CUỐI GAME\n\nNHẬN XÉT MIL\n");
            text.AppendLine(BuildMediaLiteracyInsight(gameSession));
            text.Append("\nNHẬN XÉT QUAN HỆ\n").Append(BuildRelationshipInsight(gameSession));

            return text.ToString().TrimEnd();
        }

        private static string BuildMediaLiteracyInsight(GameSession gameSession)
        {
            MediaLiteracyMetric[] metrics = (MediaLiteracyMetric[])Enum.GetValues(typeof(MediaLiteracyMetric));
            int highestScore = metrics.Max(gameSession.MediaLiteracy.GetScore);
            MediaLiteracyMetric[] highestMetrics = metrics
                .Where(metric => gameSession.MediaLiteracy.GetScore(metric) == highestScore).ToArray();

            if (highestMetrics.Length != 1)
                return "Người ra quyết định cân bằng\nCác lựa chọn của bạn không quá nghiêng về một nguyên tắc MIL cụ thể. Bạn có xu hướng cân nhắc bằng chứng, con người, tính minh bạch, quyền riêng tư và mức độ khẩn cấp tùy theo từng tình huống. Đây là một cách tiếp cận linh hoạt, vì nhiều quyết định về truyền thông đòi hỏi phải cân bằng nhiều giá trị khác nhau.";

            return highestMetrics[0] switch
            {
                MediaLiteracyMetric.Evidence => "Người kiểm chứng thông tin\nBạn có xu hướng dừng lại và kiểm tra xem thông tin có thực sự đáng tin cậy trước khi tin hoặc chia sẻ. Bạn coi trọng nguồn tin, bối cảnh và bằng chứng hơn những ấn tượng ban đầu. Đây là một kỹ năng quan trọng để đối phó với thông tin sai lệch, nhưng hãy nhớ rằng bằng chứng không phải lúc nào cũng phản ánh đầy đủ tác động của thông tin đối với con người.",
                MediaLiteracyMetric.Empathy => "Người giao tiếp lấy con người làm trung tâm\nBạn thường cân nhắc thông tin và quyết định của mình sẽ ảnh hưởng đến người khác như thế nào. Bạn coi trọng phẩm giá, sự đồng thuận và những tổn hại có thể xảy ra. Tuy nhiên, sự đồng cảm vẫn cần đi cùng với việc kiểm chứng thông tin cẩn thận.",
                MediaLiteracyMetric.Transparency => "Người giao tiếp minh bạch\nBạn có xu hướng đề cao sự rõ ràng và trách nhiệm giải trình thay vì che giấu thông tin hoặc sự không chắc chắn. Minh bạch có thể xây dựng lòng tin, nhưng không phải mọi thông tin đều nên được công khai. Minh bạch có trách nhiệm cũng đòi hỏi sự tôn trọng quyền riêng tư.",
                MediaLiteracyMetric.Privacy => "Người bảo vệ quyền riêng tư\nBạn thận trọng khi quyết định ai nên được tiếp cận thông tin và liệu người liên quan đã đồng ý cho thông tin đó được sử dụng hay chưa. Bạn hiểu rằng một thông tin có thể được tiếp cận không có nghĩa là nó nên được chia sẻ. Tuy nhiên, quyền riêng tư đôi khi cũng cần được cân nhắc cùng với lợi ích chính đáng của cộng đồng.",
                MediaLiteracyMetric.Crisis => "Người ứng phó khủng hoảng\nKhi chịu áp lực, bạn ưu tiên giảm thiểu nguy cơ trước mắt và truyền đạt những thông tin cần thiết một cách hiệu quả. Bạn nhận ra rằng trong khủng hoảng, tốc độ và cách truyền tải thông tin có thể ảnh hưởng trực tiếp đến hành động của mọi người. Tuy nhiên, sự khẩn cấp không nên thay thế việc kiểm chứng thông tin.",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string BuildRelationshipInsight(GameSession gameSession)
        {
            var scores = gameSession.Relationships.Scores.ToArray();
            if (scores.Length == 0) return "Chưa có điểm quan hệ nào được ghi nhận.";

            int highestScore = scores.Max(x => x.Value);
            return string.Join("\n\n", scores
                .Where(x => x.Value == highestScore)
                .OrderBy(x => x.Key.Value, StringComparer.Ordinal)
                .Select(x => GetRelationshipInsight(x.Key.Value)));
        }

        private static string GetRelationshipInsight(string characterName) => characterName switch
            {
                "Cô Hương" => "Nhận được sự tin tưởng của Cô Hương\nCác quyết định của bạn phù hợp nhất với cách Cô Hương nhìn nhận tình huống. Qua những lựa chọn của bạn, cô ngày càng tin tưởng vào khả năng xử lý những thông tin nhạy cảm của bạn.",
                "Linh" => "Xây dựng niềm tin với Linh\nLinh phản ứng tích cực nhất với những quyết định của bạn. Các lựa chọn của bạn khiến Linh sẵn sàng tin tưởng vào đánh giá của bạn hơn khi phải đối mặt với những thông tin khó xử lý.",
                "Minh" => "Thuyết phục được Minh\nCách tiếp cận của bạn phù hợp nhất với quan điểm của Minh. Ngay cả khi tình huống chưa rõ ràng, những quyết định của bạn khiến Minh ngày càng tin tưởng vào cách bạn xử lý vấn đề.",
                "Vy" => "Tạo được sự đồng cảm với Vy\nCác lựa chọn của bạn gần nhất với những điều Vy coi trọng. Trong quá trình giải quyết các tình huống, Vy ngày càng sẵn sàng lắng nghe và tin tưởng vào quan điểm của bạn.",
                _ => $"{characterName} là người tin tưởng nhất vào cách bạn đưa ra quyết định."
            };

        private void PopulateFinalSnapshot(GameSession gameSession)
        {
            if (_snapshotContainer == null) return;
            foreach (Transform child in _snapshotContainer) Destroy(child.gameObject);

            foreach (MediaLiteracyMetric metric in (MediaLiteracyMetric[])Enum.GetValues(typeof(MediaLiteracyMetric)))
            {
                int score = gameSession.MediaLiteracy.GetScore(metric);
                RuntimeUIFactory.CreateText(_snapshotContainer, metric.ToString(), $"{metric}: {score}", 22, TextAnchor.MiddleCenter,
                    score >= 0 ? new Color(0.6f, 0.85f, 0.6f) : new Color(0.9f, 0.6f, 0.6f));
            }
            foreach (KeyValuePair<CharacterId, int> relationship in gameSession.Relationships.Scores.OrderBy(x => x.Key.Value, StringComparer.Ordinal))
            {
                RuntimeUIFactory.CreateText(_snapshotContainer, relationship.Key.Value, $"{relationship.Key.Value}: {relationship.Value}", 22, TextAnchor.MiddleCenter,
                    relationship.Value >= 0 ? new Color(0.6f, 0.75f, 0.95f) : new Color(0.9f, 0.6f, 0.6f));
            }
        }

        private void OnPlayAgainClicked()
        {
            Hide();
            _onPlayAgain?.Invoke();
        }

        private void OnShareExperienceClicked()
        {
            Hide();
            _onShareExperience?.Invoke();
        }

        private void OnReviewJourneyClicked()
        {
            if (_journeyPanel == null || _lastSession == null) return;
            _journeyExpanded = !_journeyExpanded;
            if (_journeyExpanded)
            {
                _journeyText.text = _lastSession.ChoiceHistory.SelectedChoiceIds.Count == 0
                    ? "Bạn chưa đưa ra lựa chọn nào."
                    : string.Join("\n", _lastSession.ChoiceHistory.SelectedChoiceIds.Select(id => $"• {id.Value}"));
            }
            _journeyPanel.SetActive(_journeyExpanded);
        }

        private void Hide()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
