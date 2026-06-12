using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IAdminService
    {
        //слова
        Task<IEnumerable<WordDto>> GetAllWordsAsync();
        Task<WordDto> GetWordByIdAsync(long id);
        Task<WordDto> AddWordAsync(AddWordDto dto);
        Task<bool> DeleteWordAsync(long id);

        //граммар уроки
        Task<IEnumerable<GrammarTopicDto>> GetAllGrammarTopicsAsync();
        Task<GrammarTopicDto> GetGrammarTopicByIdAsync(long id);
        Task<GrammarTopicDto> AddGrammarTopicAsync(AddGrammarTopicDto dto);
        Task<GrammarTopicDto> UpdateGrammarTopicAsync(long id, UpdateGrammarTopicDto dto);
        Task<bool> DeleteGrammarTopicAsync(long id);

        //граммар тесты
        Task<IEnumerable<GrammarTestDto>> GetAllGrammarTestsAsync();
        Task<GrammarTestDto> GetGrammarTestByIdAsync(long id);
        Task<IEnumerable<GrammarTestDto>> GetGrammarTestsByTopicIdAsync(long topicId);
        Task<GrammarTestDto> AddGrammarTestAsync(AddGrammarTestDto dto);
        Task<GrammarTestDto> UpdateGrammarTestAsync(long id, UpdateGrammarTestDto dto);
        Task<bool> DeleteGrammarTestAsync(long id);

        //вокаб уроки
        Task<IEnumerable<VocabTopicDto>> GetAllVocabLessonsAsync();
        Task<VocabTopicDto> GetVocabLessonByIdAsync(long id);
        Task<VocabTopicDto> AddVocabLessonAsync(AddVocabLessonDto dto);
        Task<VocabTopicDto> UpdateVocabLessonAsync(long id, UpdateVocabLessonDto dto);
        Task<bool> DeleteVocabLessonAsync(long id);

        //вокаб тесты
        Task<IEnumerable<VocabTestDto>> GetAllVocabTestsAsync();
        Task<VocabTestDto> GetVocabTestByIdAsync(long id);
        Task<IEnumerable<VocabTestDto>> GetVocabTestsByLessonIdAsync(long lessonId);
        Task<VocabTestDto> AddVocabTestAsync(AddVocabTestDto dto);
        Task<VocabTestDto> UpdateVocabTestAsync(long id, UpdateVocabTestDto dto);
        Task<bool> DeleteVocabTestAsync(long id);
    }
}