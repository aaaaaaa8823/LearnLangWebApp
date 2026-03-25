namespace LearnEnglishWebApp.DTOs.Response
{
    public class WordDto
    {
        public long Id { get; set; }
        public string Word {  get; set; }
        public string Translation { get; set; }
        public string Definition {  get; set; }
        public string PartOfSpeech {  get; set; }
        public List<string> Examples { get; set; }
    }
}
