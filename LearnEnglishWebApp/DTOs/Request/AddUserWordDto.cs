namespace LearnEnglishWebApp.DTOs.Request
{
    public class AddUserWordDto
    {
        public long UserId { get; set; }
        public long WordId { get; set; }
        public string Status { get; set; } = "learning";
    }
}
