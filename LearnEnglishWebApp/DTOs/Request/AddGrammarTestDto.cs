namespace LearnEnglishWebApp.DTOs.Request
{
    public class AddGrammarTestDto
    {
        public long GrammarTopicId { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public int QuestionCount { get; set; }
        public int PassingScore { get; set; }
        public int TimeLimitMinutes { get; set; }
        public int OrderIndex { get; set; }
        public string? QuestionsText { get; set; }   
        public string? AnswersText { get; set; }
    }

    public class UpdateGrammarTestDto : AddGrammarTestDto { }
}
