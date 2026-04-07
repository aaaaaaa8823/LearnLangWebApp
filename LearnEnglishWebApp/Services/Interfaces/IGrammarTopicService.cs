using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IGrammarTopicService
    {
        Task<IEnumerable<GrammarTopicDto>> GetAllTopicsAsync();
        Task<GrammarTopicDto> GetTopicByIdAsync(long id);
        Task<IEnumerable<GrammarTopicDto>> GetTopicsByLevelAsync(string level);
        Task<IEnumerable<GrammarTopicDto>> GetUserTopicsWithProgressAsync(long userId);
        Task<UserGrammarProgressDto> MarkTopicAsCompletedAsync(long userId, long topicId);
        Task<UserGrammarProgressDto> GetUserProgressAsync(long userId, long topicId);
        Task<IEnumerable<GrammarTopicDto>> GetAvailableTopicsForUserAsync(long userId);

    }
}
