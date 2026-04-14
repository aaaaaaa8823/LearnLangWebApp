using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IUserLessonService
    {
        Task<SavedLessonDto> SaveLessonAsync(long userId, long lessonId, string lessonType);
        Task<bool> RemoveSaveLessonAsync(long userId, long lessonId, string lessonType);
        Task<IEnumerable<SavedLessonDto>> GetSaveLessonsAsync(long userId);
        Task<bool> IsLessonSaveAsync(long userId, long lessonId, string lessonType);
    }
}
