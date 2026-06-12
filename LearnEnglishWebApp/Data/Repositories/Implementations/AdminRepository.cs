using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class AdminRepository: IAdminRepository
    {
        private readonly AppDbContext _context;
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        //слова
        public async Task<IEnumerable<Dictionary>> GetAllWordsAsync()
        {
            return await _context.Dictionaries
               .OrderBy(w => w.Word)
               .ToListAsync();
        }

        public async Task<Dictionary> GetWordByIdAsync(long id)
        {
            return await _context.Dictionaries.FindAsync(id);
        }

        public async Task<Dictionary> GetWordByTextAsync(string word)
        {
            return await _context.Dictionaries
                .FirstOrDefaultAsync(w => w.Word == word.ToLower());
        }

        public async Task AddWordAsync(Dictionary word)
        {
            await _context.Dictionaries.AddAsync(word);
        }

        public void UpdateWord(Dictionary word)
        {
            _context.Dictionaries.Update(word);
        }

        public void DeleteWord(Dictionary word)
        {
            _context.Dictionaries.Remove(word);
        }

        //уроки
        public async Task<IEnumerable<GrammarTopic>> GetAllGrammarTopicsAsync()
        {
            return await _context.GrammarTopics
                .Include(t => t.GrammarTests)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();
        }

        public async Task<GrammarTopic> GetGrammarTopicByIdAsync(long id)
        {
            return await _context.GrammarTopics
                .Include(t => t.GrammarTests)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddGrammarTopicAsync(GrammarTopic topic)
        {
            await _context.GrammarTopics.AddAsync(topic);
        }

        public void UpdateGrammarTopic(GrammarTopic topic)
        {
            _context.GrammarTopics.Update(topic);
        }

        public void DeleteGrammarTopic(GrammarTopic topic)
        {
            _context.GrammarTopics.Remove(topic);
        }

        public async Task<IEnumerable<VocabLesson>> GetAllVocabLessonsAsync()
        {
            return await _context.VocabLessons
                .Include(l => l.VocabTests)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync();
        }

        public async Task<VocabLesson> GetVocabLessonByIdAsync(long id)
        {
            return await _context.VocabLessons
                .Include(l => l.VocabTests)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddVocabLessonAsync(VocabLesson lesson)
        {
            await _context.VocabLessons.AddAsync(lesson);
        }

        public void UpdateVocabLesson(VocabLesson lesson)
        {
            _context.VocabLessons.Update(lesson);
        }

        public void DeleteVocabLesson(VocabLesson lesson)
        {
            _context.VocabLessons.Remove(lesson);
        }

        //тесты
        public async Task<IEnumerable<GrammarTest>> GetAllGrammarTestsAsync()
        {
            return await _context.GrammarTests
                .Include(t => t.GrammarTopic)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();
        }

        public async Task<GrammarTest> GetGrammarTestByIdAsync(long id)
        {
            return await _context.GrammarTests
                .Include(t => t.GrammarTopic)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<GrammarTest>> GetGrammarTestsByTopicIdAsync(long topicId)
        {
            return await _context.GrammarTests
                .Where(t => t.GrammarTopicId == topicId)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();
        }

        public async Task AddGrammarTestAsync(GrammarTest test)
        {
            await _context.GrammarTests.AddAsync(test);
        }

        public void UpdateGrammarTest(GrammarTest test)
        {
            _context.GrammarTests.Update(test);
        }

        public void DeleteGrammarTest(GrammarTest test)
        {
            _context.GrammarTests.Remove(test);
        }

        public async Task<IEnumerable<VocabTest>> GetAllVocabTestsAsync()
        {
            return await _context.VocabTests
                .Include(t => t.VocabLesson)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();
        }

        public async Task<VocabTest> GetVocabTestByIdAsync(long id)
        {
            return await _context.VocabTests
                .Include(t => t.VocabLesson)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<VocabTest>> GetVocabTestsByLessonIdAsync(long lessonId)
        {
            return await _context.VocabTests
                .Where(t => t.VocabLessonId == lessonId)
                .OrderBy(t => t.OrderIndex)
                .ToListAsync();
        }

        public async Task AddVocabTestAsync(VocabTest test)
        {
            await _context.VocabTests.AddAsync(test);
        }

        public void UpdateVocabTest(VocabTest test)
        {
            _context.VocabTests.Update(test);
        }

        public void DeleteVocabTest(VocabTest test)
        {
            _context.VocabTests.Remove(test);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
