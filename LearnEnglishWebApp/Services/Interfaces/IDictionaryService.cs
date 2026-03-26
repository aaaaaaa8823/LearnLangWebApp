using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IDictionaryService
    {
        Task<IEnumerable<WordDto>> GetAllWordAsync();
        Task<IEnumerable<WordDto>> SearchWordAsync(string searchTerm);
        Task<WordDto> GetWordById(long id);
        Task<IEnumerable<WordDto>> GetWordByLevelAsync(string level);

    }
}
