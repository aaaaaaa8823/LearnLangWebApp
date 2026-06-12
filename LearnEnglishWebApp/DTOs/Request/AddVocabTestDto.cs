public class AddVocabTestDto
{
    public long VocabLessonId { get; set; }
    public string Title { get; set; }
    public string Level { get; set; }
    public string Description { get; set; }
    public string TestType { get; set; } //
    public int QuestionCount { get; set; }
    public int PassingScore { get; set; }
    public int TimeLimitMinutes { get; set; }
    public int OrderIndex { get; set; }
}

public class UpdateVocabTestDto : AddVocabTestDto { }