using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class GrammarTopicRepository : IGrammarTopicRepository
    {

        private readonly AppDbContext _context;

        public GrammarTopicRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.GrammarTopics.AnyAsync(gt => gt.Id == id);
        }

        public async Task<IEnumerable<GrammarTopic>> GetAllTopicAsync()
        {
          return await _context.GrammarTopics.Include(gt => gt.GrammarTests)
                .Include(gt => gt.UserGrammarProgresses)
                .OrderBy(gt => gt.OrderIndex).ToListAsync();
        }

        public async Task<IEnumerable<GrammarTopic>> GetByLevelAsync(string level)
        {
            return await _context.GrammarTopics.Include(gt => gt.GrammarTests)
                .Include(gt => gt.UserGrammarProgresses)
                .Where(gt => gt.Level == level).OrderBy(gt => gt.OrderIndex).ToListAsync();
        }

        public async Task<IEnumerable<GrammarTopic>> GetByUserProgressAsync(long userId, bool? completed = null)
        {
            var query = _context.GrammarTopics
                 .Include(gt => gt.GrammarTests)
                 .Include(gt => gt.UserGrammarProgresses)
                 .OrderBy(gt => gt.OrderIndex);

            if (completed.HasValue)
            {
                return await query
                    .Where(gt => gt.UserGrammarProgresses.Any(ugp => ugp.UserId == userId && ugp.Completed == completed.Value))
                    .ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<GrammarTopic> GetTopicByIdAsync(long id)
        {
            return await _context.GrammarTopics.Include(gt => gt.GrammarTests)
                 .Include(gt => gt.UserGrammarProgresses).FirstOrDefaultAsync(gt => gt.Id == id);
        }
    }
}
