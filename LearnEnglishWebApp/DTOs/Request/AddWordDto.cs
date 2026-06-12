namespace LearnEnglishWebApp.DTOs.Request
{
    public class AddWordDto
    {
        public string Word { get; set; }
        public string Translation { get; set; }
        public string Definition { get; set; }
        public string PartOfSpeech { get; set; }
        public string DifficultyLevel { get; set; }
        public List<string> Examples { get; set; }
    }

    public class UpdateWordDto : AddWordDto { }
}
