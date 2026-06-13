namespace LearnEnglishWebApp.DTOs.Response
{
    public class VocabTopicDto
    {
        public long Id { get; set; }
        public string Level { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string TheoryContent { get; set; }
        public string ExamplesContent { get; set; }
        public List<VocabTestDto> Tests { get; set; } = new List<VocabTestDto>();
    }

    public class VocabTestDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string TestType { get; set; }
        public int QuestionCount { get; set; }
        public int PassingScore { get; set; }
        public int TimeLimitMinutes { get; set; }

        public bool IsActive { get; set; }
    }

    public class UserVocabProgressDto
    {
        public long UserId { get; set; }
        public long LessonId { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? Score { get; set; }
        public string LessonTitle { get; set; }
    }
}
