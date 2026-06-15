namespace LearnEnglishWebApp.DTOs.Response
{
    public class GrammarTestAdminDto
    {
        public long Id { get; set; }
        public long GrammarTopicId { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public int QuestionCount { get; set; }
        public int PassingScore { get; set; }
        public int TimeLimitMinutes { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? QuestionsText { get; set; }
        public string? AnswersText { get; set; }
        public GrammarTopicSimpleDto? GrammarTopic { get; set; }
    }

    public class GrammarTopicSimpleDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
    }
}