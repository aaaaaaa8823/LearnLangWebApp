namespace LearnEnglishWebApp.DTOs.Request
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Level { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
