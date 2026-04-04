namespace LearnEnglishWebApp.DTOs.Response
{
    public class UserWordDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long WordId {  get; set; }
        public string Word { get; set; }
        public string Translation { get; set; }
        public string Status { get; set; }
        public DateTime AddedAt {  get; set; }
    }
}
