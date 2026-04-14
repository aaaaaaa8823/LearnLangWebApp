using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IUserLessonService
    {
        Task<SavedLessonDto> SaveLessonAsync(long userId, long lessonId, string lessonType);
        Task<bool> RemoveSavedLessonAsync(long userId, long lessonId, string lessonType);
        Task<IEnumerable<SavedLessonDto>> GetSavedLessonsAsync(long userId);
        Task<bool> IsLessonSavedAsync(long userId, long lessonId, string lessonType);
    }
}
