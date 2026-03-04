using LearnEnglishWebApp.DTOs.Request;

namespace LearnEnglishWebApp.DTOs.Response
{
    public class AuthResponseDto
    {
        public string Token {  get; set; }
        public UserDto User { get; set; }
        public DateTime ResponseLenght {  get; set; }

    }
}
