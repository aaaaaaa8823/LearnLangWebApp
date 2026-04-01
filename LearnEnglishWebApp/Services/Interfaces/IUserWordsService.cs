using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IUserWordsService
    {
        Task<UserWordDto> AddWordToUserAsync(long userId, long wordId, string status);
        Task<IEnumerable<UserWordDto>> GetUserWordsAsync(long userId, string status = null);
        Task<bool> RemoveWordFromUserAsync(long userId, long wordId);
        Task<UserWordDto> UpdateWordStatusAsync(long userId, long wordId, string newStatus);
    }
}
