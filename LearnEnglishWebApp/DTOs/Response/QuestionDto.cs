namespace LearnEnglishWebApp.DTOs.Response
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOption { get; set; }
        public int Points { get; set; }
    }
}