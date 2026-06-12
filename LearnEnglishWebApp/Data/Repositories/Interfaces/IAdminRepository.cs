using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        //слова
        Task<IEnumerable<Dictionary>> GetAllWordsAsync();
        Task<Dictionary> GetWordByIdAsync(long id);
        Task<Dictionary> GetWordByTextAsync(string word);
        Task AddWordAsync(Dictionary word);
        void UpdateWord(Dictionary word);
        void DeleteWord(Dictionary word);

        //уроки
        Task<IEnumerable<GrammarTopic>> GetAllGrammarTopicsAsync();
        Task<GrammarTopic> GetGrammarTopicByIdAsync(long id);
        Task AddGrammarTopicAsync(GrammarTopic topic);
        void UpdateGrammarTopic(GrammarTopic topic);
        void DeleteGrammarTopic(GrammarTopic topic);

        Task<IEnumerable<VocabLesson>> GetAllVocabLessonsAsync();
        Task<VocabLesson> GetVocabLessonByIdAsync(long id);
        Task AddVocabLessonAsync(VocabLesson lesson);
        void UpdateVocabLesson(VocabLesson lesson);
        void DeleteVocabLesson(VocabLesson lesson);
        
        //тесты
        Task<IEnumerable<GrammarTest>> GetAllGrammarTestsAsync();
        Task<GrammarTest> GetGrammarTestByIdAsync(long id);
        Task<IEnumerable<GrammarTest>> GetGrammarTestsByTopicIdAsync(long topicId);
        Task AddGrammarTestAsync(GrammarTest test);
        void UpdateGrammarTest(GrammarTest test);
        void DeleteGrammarTest(GrammarTest test);

        Task<IEnumerable<VocabTest>> GetAllVocabTestsAsync();
        Task<VocabTest> GetVocabTestByIdAsync(long id);
        Task<IEnumerable<VocabTest>> GetVocabTestsByLessonIdAsync(long lessonId);
        Task AddVocabTestAsync(VocabTest test);
        void UpdateVocabTest(VocabTest test);
        void DeleteVocabTest(VocabTest test);

        // Сохранение изменений
        Task<int> SaveChangesAsync();
    }
}
