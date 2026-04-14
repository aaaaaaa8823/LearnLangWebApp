namespace LearnEnglishWebApp.DTOs.Response
{
    public class SavedTestDto
    {
        public long Id { get; set; }
        public long TestId { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string TestType { get; set; } 
        public int QuestionCount { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
