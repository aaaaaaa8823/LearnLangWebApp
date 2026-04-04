namespace LearnEnglishWebApp.DTOs.Request
{
    public class UpdateWordStatusDto
    {
        public long UserId { get; set; }
        public long WordId { get; set; }
        public string Status { get; set; }
    }
}
