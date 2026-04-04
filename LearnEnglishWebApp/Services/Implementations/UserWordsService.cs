using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;    

namespace LearnEnglishWebApp.Services.Implementations
{
    public class UserWordsService : IUserWordsService
    {
        private readonly IUserWordsRepository _userWordsRepository;
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<UserWordsService> _logger;

        public UserWordsService(
           IUserWordsRepository userWordRepository,
           IDictionaryRepository dictionaryRepository,
           IUserRepository userRepository,
           AppDbContext context,
           ILogger<UserWordsService> logger)
        {
            _userWordsRepository = userWordRepository;
            _dictionaryRepository = dictionaryRepository;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<UserWordDto> AddWordToUserAsync(long userId, long wordId, string status)
        {
            try
            {
                _logger.LogInformation("Начинаем добавление слова: UserId={UserId}, WordId={WordId}, Status={Status}", userId, wordId, status);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь не найден: UserId={UserId}", userId);
                    throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
                }
                _logger.LogInformation("Пользователь найден: {UserName}", user.UserName);

                var word = await _dictionaryRepository.GetByIdAsync(wordId);
                if (word == null)
                {
                    _logger.LogWarning("Слово не найдено: WordId={WordId}", wordId);
                    throw new InvalidOperationException($"Слово с ID {wordId} не найдено");
                }
                _logger.LogInformation("Слово найдено: {Word}", word.Word);

                var exists = await _userWordsRepository.ExistsAsync(userId, wordId);
                if (exists)
                {
                    _logger.LogWarning("Слово уже добавлено пользователю: UserId={UserId}, WordId={WordId}", userId, wordId);
                    throw new InvalidOperationException("Это слово уже добавлено");
                }

                var userWord = new UserWord
                {
                    UserId = userId,
                    WordId = wordId,
                    Status = status,
                    AddedAt = DateTime.UtcNow,
                    ContextSentence = ""
                };

                _logger.LogInformation("Добавляем запись в UserWords");
                await _userWordsRepository.AddAsync(userWord);

                _logger.LogInformation("Сохраняем изменения в БД");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Слово успешно добавлено!");

                return new UserWordDto
                {
                    Id = userWord.Id,
                    UserId = userWord.UserId,
                    WordId = userWord.WordId,
                    Word = word.Word,
                    Translation = word.Translation,
                    Status = userWord.Status,
                    AddedAt = userWord.AddedAt
                };
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Ошибка БД при добавлении слова. Внутреннее исключение: {InnerException}", dbEx.InnerException?.Message);

                if (dbEx.InnerException != null)
                {
                    throw new InvalidOperationException($"Ошибка БД: {dbEx.InnerException.Message}");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Общая ошибка при добавлении слова пользователю");
                throw;
            }
        }

        public async Task<IEnumerable<UserWordDto>> GetUserWordsAsync(long userId, string status = null)
        {
            var userWords = await _userWordsRepository.GetByUserIdAsync(userId, status);

            return userWords.Select(uw => new UserWordDto
            {
                Id = uw.Id,
                UserId = uw.UserId,
                WordId = uw.WordId,
                Word = uw.Word?.Word ?? "Неизвестно",
                Translation = uw.Word?.Translation ?? "",
                Status = uw.Status,
                AddedAt = uw.AddedAt
            });

        }

        public async Task<bool> RemoveWordFromUserAsync(long userId, long wordId)
        {
            try
            {
                var userWord = await _userWordsRepository.GetByUserAndWordAsync(userId, wordId);
                if (userWord == null)
                    return false;

                _userWordsRepository.Delete(userWord);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Слово удалено у пользователя: UserId={UserId}, WordId={WordId}", userId, wordId);
                return true;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при удалении слова у пользователя");
                throw;
            }
        }

        public async Task<UserWordDto> UpdateWordStatusAsync(long userId, long wordId, string newStatus)
        {
            try
            {
                var userWord = await _userWordsRepository.GetByUserAndWordAsync(userId, wordId);

                if (userWord == null)
                    throw new InvalidOperationException("Слово не найдено в словаре пользователя");

                userWord.Status = newStatus;
                _userWordsRepository.Update(userWord);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Статус слова обновлен: UserId={UserId}, WordId={WordId}, NewStatus={NewStatus}",
                    userId, wordId, newStatus);

                return new UserWordDto
                {
                    Id = userWord.Id,
                    UserId = userWord.UserId,
                    WordId = userWord.WordId,
                    Word = userWord.Word?.Word ?? "Неизвестно",
                    Translation = userWord.Word?.Translation ?? "",
                    Status = userWord.Status,
                    AddedAt = userWord.AddedAt
                };
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при обновлении статуса слова");
                throw;
            }
        }
    }
}
