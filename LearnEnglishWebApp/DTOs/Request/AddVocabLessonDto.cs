namespace LearnEnglishWebApp.DTOs.Request
{
    public class AddVocabLessonDto
    {
        public string Level { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }
        public string TheoryContent { get; set; }
        public string ExamplesContent { get; set; }
    }
    public class UpdateVocabLessonDto : AddVocabLessonDto { }
}
