using System;
using System.Linq;
using System.Text;
using Bedrot.Narrative.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Bedrot.Narrative.Presentation
{
    public sealed class FinalInsightViewModel
    {
        public string ScreenTitle { get; }
        public string MediaLiteracyPanelLabel { get; }
        public string MediaLiteracyInsightTitle { get; }
        public string MediaLiteracyInsightBody { get; }
        public string RelationshipPanelLabel { get; }
        public string RelationshipInsightTitle { get; }
        public string RelationshipInsightBody { get; }
        public string RelationshipCharacterId { get; }

        public FinalInsightViewModel(
            string screenTitle,
            string mediaLiteracyPanelLabel,
            string mediaLiteracyInsightTitle,
            string mediaLiteracyInsightBody,
            string relationshipPanelLabel,
            string relationshipInsightTitle,
            string relationshipInsightBody,
            string relationshipCharacterId)
        {
            ScreenTitle = screenTitle;
            MediaLiteracyPanelLabel = mediaLiteracyPanelLabel;
            MediaLiteracyInsightTitle = mediaLiteracyInsightTitle;
            MediaLiteracyInsightBody = mediaLiteracyInsightBody;
            RelationshipPanelLabel = relationshipPanelLabel;
            RelationshipInsightTitle = relationshipInsightTitle;
            RelationshipInsightBody = relationshipInsightBody;
            RelationshipCharacterId = relationshipCharacterId;
        }
    }

    [Serializable]
    public sealed class RelationshipPortraitEntry
    {
        [SerializeField] private string characterId;
        [SerializeField] private Sprite portrait;

        public string CharacterId => characterId;
        public Sprite Portrait => portrait;
    }

    /// <summary>Presents the terminal screen after the final narrative scene.</summary>
    public sealed class GameEndingPresenter : MonoBehaviour
    {
        [Header("Visibility")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject viewRoot;
        [SerializeField] private bool hideOnAwake = true;

        [Header("Top frame")]
        [SerializeField] private Text screenTitleText;

        [Header("Left panel - MIL insight")]
        [SerializeField] private Text mediaLiteracyPanelLabelText;
        [SerializeField] private Text mediaLiteracyInsightTitleText;
        [SerializeField] private Text mediaLiteracyInsightBodyText;

        [Header("Right panel - relationship insight")]
        [SerializeField] private Text relationshipPanelLabelText;
        [SerializeField] private Text relationshipInsightTitleText;
        [SerializeField] private Text relationshipInsightBodyText;
        [SerializeField] private Image relationshipPortraitImage;
        [SerializeField] private RelationshipPortraitEntry[] relationshipPortraits = Array.Empty<RelationshipPortraitEntry>();

        [Header("Optional legacy fallback")]
        [SerializeField] private Text legacyMessageText;

        private Action _onPlayAgain;
        private Action _onShareExperience;
        private GameSession _lastSession;
        private GameObject _journeyPanel;
        private Text _journeyText;
        private bool _journeyExpanded;
        private bool _actionButtonsBuilt;

        private void Awake()
        {
            if (hideOnAwake) Hide();
        }

        /// <summary>Binds the "Play Again" and "Share Experience" callbacks, dispatched by the action buttons this presenter builds at runtime.</summary>
        public void Bind(Action onPlayAgain, Action onShareExperience)
        {
            _onPlayAgain = onPlayAgain;
            _onShareExperience = onShareExperience;
        }

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
            Stretch(message.GetComponent<RectTransform>());
            Text text = message.GetComponent<Text>();
            text.text = string.Empty;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 48;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 20;
            text.resizeTextMaxSize = 48;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameEndingPresenter presenter = root.GetComponent<GameEndingPresenter>();
            presenter.canvasGroup = root.GetComponent<CanvasGroup>();
            presenter.viewRoot = root;
            presenter.legacyMessageText = text;
            presenter.Hide();
            return presenter;
        }

        public void ShowResults(GameSession gameSession)
        {
            if (gameSession == null) throw new ArgumentNullException(nameof(gameSession));

            _lastSession = gameSession;
            gameObject.SetActive(true);
            if (viewRoot != null) viewRoot.SetActive(true);
            ResolveCanvasGroup();
            EnsureActionButtons();
            _journeyExpanded = false;
            if (_journeyPanel != null) _journeyPanel.SetActive(false);

            FinalInsightViewModel viewModel = BuildFinalInsightViewModel(gameSession);
            PresentFinalInsightViewModel(viewModel);

            if (canvasGroup == null) return;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public static FinalInsightViewModel BuildFinalInsightViewModel(GameSession gameSession)
        {
            if (gameSession == null) throw new ArgumentNullException(nameof(gameSession));

            SplitInsight(BuildMediaLiteracyInsight(gameSession), out string mediaTitle, out string mediaBody);

            string relationshipInsight = BuildRelationshipInsight(gameSession);
            string relationshipCharacterId = GetSingleHighestRelationshipCharacterId(gameSession);
            if (relationshipCharacterId == null && gameSession.Relationships.Scores.Count > 1)
            {
                return new FinalInsightViewModel(
                    "NHẬN XÉT CUỐI GAME",
                    "NHẬN XÉT MIL",
                    mediaTitle,
                    mediaBody,
                    "NHẬN XÉT QUAN HỆ",
                    "Nhiều mối quan hệ nổi bật",
                    relationshipInsight,
                    null);
            }

            SplitInsight(relationshipInsight, out string relationshipTitle, out string relationshipBody);
            return new FinalInsightViewModel(
                "NHẬN XÉT CUỐI GAME",
                "NHẬN XÉT MIL",
                mediaTitle,
                mediaBody,
                "NHẬN XÉT QUAN HỆ",
                relationshipTitle,
                relationshipBody,
                relationshipCharacterId);
        }

        private void PresentFinalInsightViewModel(FinalInsightViewModel viewModel)
        {
            SetText(screenTitleText, viewModel.ScreenTitle);
            SetText(mediaLiteracyPanelLabelText, viewModel.MediaLiteracyPanelLabel);
            SetText(mediaLiteracyInsightTitleText, viewModel.MediaLiteracyInsightTitle);
            SetText(mediaLiteracyInsightBodyText, viewModel.MediaLiteracyInsightBody);
            SetText(relationshipPanelLabelText, viewModel.RelationshipPanelLabel);
            SetText(relationshipInsightTitleText, viewModel.RelationshipInsightTitle);
            SetText(relationshipInsightBodyText, viewModel.RelationshipInsightBody);
            SetText(legacyMessageText, BuildLegacyResultsText(viewModel));
            PresentRelationshipPortrait(viewModel.RelationshipCharacterId);
        }

        public static string BuildResultsText(GameSession gameSession)
        {
            if (gameSession == null) throw new ArgumentNullException(nameof(gameSession));

            var text = new StringBuilder("NHẬN XÉT CUỐI GAME\n\nNHẬN XÉT MIL\n");
            text.AppendLine(BuildMediaLiteracyInsight(gameSession));
            text.Append("\nNHẬN XÉT QUAN HỆ\n").Append(BuildRelationshipInsight(gameSession));

            return text.ToString().TrimEnd();
        }

        private static string BuildLegacyResultsText(FinalInsightViewModel viewModel)
        {
            var text = new StringBuilder(viewModel.ScreenTitle).Append("\n\n")
                .Append(viewModel.MediaLiteracyPanelLabel).Append('\n')
                .Append(viewModel.MediaLiteracyInsightTitle);
            if (!string.IsNullOrWhiteSpace(viewModel.MediaLiteracyInsightBody))
                text.Append('\n').Append(viewModel.MediaLiteracyInsightBody);

            text.Append("\n\n").Append(viewModel.RelationshipPanelLabel).Append('\n')
                .Append(viewModel.RelationshipInsightTitle);
            if (!string.IsNullOrWhiteSpace(viewModel.RelationshipInsightBody))
                text.Append('\n').Append(viewModel.RelationshipInsightBody);

            return text.ToString();
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

        private static string GetSingleHighestRelationshipCharacterId(GameSession gameSession)
        {
            var scores = gameSession.Relationships.Scores.ToArray();
            if (scores.Length == 0) return null;

            int highestScore = scores.Max(x => x.Value);
            var winners = scores.Where(x => x.Value == highestScore).ToArray();
            return winners.Length == 1 ? winners[0].Key.Value : null;
        }

        private static void SplitInsight(string insight, out string title, out string body)
        {
            string normalized = (insight ?? string.Empty).Replace("\r\n", "\n").Trim();
            int separatorIndex = normalized.IndexOf('\n');
            if (separatorIndex < 0)
            {
                title = normalized;
                body = string.Empty;
                return;
            }

            title = normalized.Substring(0, separatorIndex).Trim();
            body = normalized.Substring(separatorIndex + 1).Trim();
        }

        private void PresentRelationshipPortrait(string characterId)
        {
            if (relationshipPortraitImage == null) return;

            RelationshipPortraitEntry match = null;
            if (!string.IsNullOrWhiteSpace(characterId) && relationshipPortraits != null)
            {
                match = Array.Find(relationshipPortraits,
                    entry => entry != null && string.Equals(entry.CharacterId, characterId, StringComparison.Ordinal));
            }

            bool hasPortrait = match?.Portrait != null;
            relationshipPortraitImage.sprite = hasPortrait ? match.Portrait : null;
            relationshipPortraitImage.gameObject.SetActive(hasPortrait);
        }

        private void ResolveCanvasGroup()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value ?? string.Empty;
        }

        private void Hide()
        {
            ResolveCanvasGroup();
            if (canvasGroup == null) return;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Builds the "Play again / Review journey / Share experience" action row and journey panel at runtime,
        /// so they work whether this presenter was scene-authored or created via <see cref="Create"/>. Idempotent.
        /// </summary>
        private void EnsureActionButtons()
        {
            if (_actionButtonsBuilt) return;
            _actionButtonsBuilt = true;

            GameObject overlay = RuntimeUIFactory.CreateOverlayCanvas("EndingActionsOverlay", transform, short.MaxValue);

            var journeyPanel = new GameObject("JourneyPanel", typeof(RectTransform), typeof(Image));
            journeyPanel.transform.SetParent(overlay.transform, false);
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
            buttonRow.transform.SetParent(overlay.transform, false);
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

            _journeyPanel = journeyPanel;
            _journeyText = journeyTextComponent;
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            reviewJourneyButton.onClick.AddListener(OnReviewJourneyClicked);
            shareButton.onClick.AddListener(OnShareExperienceClicked);
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

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
