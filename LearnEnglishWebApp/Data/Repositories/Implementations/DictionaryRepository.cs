using LearnEnglishWebApp.Data.Repositories;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class DictionaryRepository: IDictionaryRepository
    {
        private readonly AppDbContext _context;

        public DictionaryRepository(AppDbContext context) { 
            _context = context;
        }


        public async Task<IEnumerable<Dictionary>> GetAllAsync()
        {
            return await _context.Dictionaries.OrderBy(d => d.Word).ToListAsync();
        }

        public async Task<Dictionary> GetByIdAsync(long id)
        {
            return await _context.Dictionaries.FindAsync(id);
        }

        public async Task<Dictionary> GetByWordAsync(string word)
        {
            return await _context.Dictionaries.FirstOrDefaultAsync(d => d.Word == word.ToLower());
        }

        public async Task<IEnumerable<Dictionary>> GetByLevelAsync(string level)
        {
            return await _context.Dictionaries.
                Where(d => d.DifficultyLevel == level).OrderBy(d => d.Word).ToListAsync();
        }

        public async Task<IEnumerable<Dictionary>> GetByPartOfSpeechAsync(string partOfSpeech)
        {
            return await _context.Dictionaries.
                Where(d => d.PartOfSpeech == partOfSpeech).OrderBy(d => d.Word).ToListAsync();
        }

        public async Task<IEnumerable<Dictionary>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _context.Dictionaries.Where(d=> d.Word.Contains(searchTerm.ToLower()) || d.Translation.Contains(searchTerm.ToLower()) 
                || (d.Definition != null && d.Definition.Contains(searchTerm))).OrderBy(d=> d.Word).ToListAsync();
        }

        public async Task AddAsync(Dictionary word)
        {
            await _context.Dictionaries.AddAsync(word);
        }

        public void Update(Dictionary word)
        {
            _context.Dictionaries.Update(word);
        }

        public void Delete(Dictionary word)
        {
            _context.Dictionaries.Remove(word);
        }
    }
}
