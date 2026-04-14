namespace LearnEnglishWebApp.DTOs.Response
{
    public class SavedLessonDto
    {
        public long Id { get; set; }
        public long LessonId { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string LessonType { get; set; } 
        public DateTime SavedAt { get; set; }
    }
}
