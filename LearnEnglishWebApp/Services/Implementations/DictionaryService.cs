using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Services.Interfaces;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class DictionaryService: IDictionaryService
    {
        private readonly IDictionaryRepository _repository;
        private readonly ILogger<DictionaryService> _logger;

        public DictionaryService(IDictionaryRepository repository, ILogger<DictionaryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<WordDto>> GetAllWordAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<WordDto> GetWordById(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WordDto>> GetWordByLevelAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WordDto>> SearchWordAsync()
        {
            throw new NotImplementedException();
        }
    }
}
