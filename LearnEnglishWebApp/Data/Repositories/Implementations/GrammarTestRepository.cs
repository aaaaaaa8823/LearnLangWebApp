using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class GrammarTestRepository : IGrammarTestRepository
    {

        private readonly AppDbContext _context;

        public GrammarTestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.GrammarTests.AnyAsync(gt => gt.Id == id);
        }


        public async Task<IEnumerable<GrammarTest>> GetAllTestAsync()
        {
            return await _context.GrammarTests.Include(gt => gt.GrammarTopic).Where(gt => gt.IsActive).OrderBy(gt => gt.OrderIndex)
                .ToListAsync();
        }

        public async Task<IEnumerable<GrammarTest>> GetByTopicIdAsync(long topicId)
        {
            return await _context.GrammarTests
                 .Include(gt => gt.GrammarTopic).Where(gt => gt.GrammarTopicId == topicId && gt.IsActive)
                 .OrderBy(gt => gt.OrderIndex).ToListAsync();
        }

        public async Task<IEnumerable<GrammarTest>> GetByLevelAsync(string level)
        {
            return await _context.GrammarTests
                .Include(gt => gt.GrammarTopic).Where(gt => gt.Level == level && gt.IsActive)
                .OrderBy(gt => gt.OrderIndex).ToListAsync();
        }

        public async Task<IEnumerable<GrammarTest>> GetByUserProgressAsync(long userId, bool? passed = null)
        {
            var query = _context.GrammarTests
                 .Include(gt => gt.GrammarTopic)
                 .Where(gt => gt.IsActive);

            if (passed.HasValue)
            {
                var testResults = await _context.TestResults
                    .Where(tr => tr.UserId == userId && tr.TestType == "grammar")
                    .Select(tr => tr.TestId)
                    .ToListAsync();

                if (passed.Value)
                {
                    query = query.Where(gt => testResults.Contains(gt.Id));
                }
                else
                {
                    query = query.Where(gt => !testResults.Contains(gt.Id));
                }
            }

            return await query.OrderBy(gt => gt.OrderIndex).ToListAsync();
        }

        public async Task<GrammarTest> GetTestByIdAsync(long id)
        {
            return await _context.GrammarTests.Include(gt => gt.GrammarTopic).
                FirstOrDefaultAsync(gt => gt.Id == id);
        }
    }
}
