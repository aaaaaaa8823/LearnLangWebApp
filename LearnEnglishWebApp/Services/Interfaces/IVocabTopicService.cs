using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IVocabTopicService
    {
        Task<IEnumerable<VocabTopicDto>> GetAllTopicsAsync();
        Task<VocabTopicDto> GetTopicByIdAsync(long id);
        Task<IEnumerable<VocabTopicDto>> GetTopicsByLevelAsync(string level);
        Task<IEnumerable<VocabTopicDto>> GetUserTopicsWithProgressAsync(long userId);
        Task<UserVocabProgressDto> MarkTopicAsCompletedAsync(long userId, long lessonId);
        Task<UserVocabProgressDto> GetUserProgressAsync(long userId, long lessonId);
    }
}