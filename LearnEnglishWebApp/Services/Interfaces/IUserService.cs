using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(long id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> UpdateLevelAsync(long userId, string newLevel);

        //Task<UserDto> GetUserWithStatisticsAsync(long userId); //дл получения в будущем полного профиля со статиской

    }
}
