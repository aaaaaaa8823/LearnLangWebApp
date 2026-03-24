namespace LearnEnglishWebApp.DTOs.Request
{
    public class UpdateProfileDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Level { get; set; }

        // Поля для смены пароля (опциональны)
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
