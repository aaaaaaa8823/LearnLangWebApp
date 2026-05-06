using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, AppDbContext context, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Level = user.Level,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                return user == null ? null : MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя по ID: {UserId}", id);
                throw;
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                return user == null ? null : MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя по email");
                throw;
            }
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("Пользователь с таким email уже существует");

                var existingUsername = await _userRepository.GetByUsernameAsync(registerDto.UserName);
                if (existingUsername != null)
                    throw new InvalidOperationException("Пользователь с таким именем уже существует");

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                var user = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    PasswordHash = passwordHash,
                    Level = "A1",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    await _userRepository.AddAsync(user);
                    await _context.SaveChangesAsync();
                    await CreateDefaultCollectionsAsync(user.Id);
                    await transaction.CommitAsync();

                    _logger.LogInformation("Пользователь успешно зарегистрирован: {Email}", user.Email);
                    return MapToDto(user);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя: {Email}", registerDto.Email);
                throw;
            }
        }

        private async Task CreateDefaultCollectionsAsync(long userId)
        {
            var collections = new[]
            {
                new Collection { UserId = userId, Name = "В процессе", IsDefault = true, CreatedAt = DateTime.UtcNow },
                new Collection { UserId = userId, Name = "Выученные", IsDefault = true, CreatedAt = DateTime.UtcNow },
            };

            await _context.Collections.AddRangeAsync(collections);
            await _context.SaveChangesAsync();
        }

        // ✅ Упрощенный Login - без JWT
        public async Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Неверный email или пароль");

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedAccessException("Неверный email или пароль");

            _logger.LogInformation("Пользователь успешно вошел: {Email}", user.Email);

            return MapToDto(user);
        }

        public async Task<UserDto> UpdateLevelAsync(long userId, string newLevel)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("Пользователь не найден");

                var validLevels = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };
                if (!validLevels.Contains(newLevel))
                    throw new ArgumentException("Недопустимый уровень");

                user.Level = newLevel;
                _userRepository.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Уровень пользователя обновлен: {UserId}, Новый уровень: {Level}", userId, newLevel);
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении уровня пользователя: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDto> UpdateProfileAsync(long userId, UpdateProfileDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("Пользователь не найден");

                if (user.Email != updateDto.Email)
                {
                    var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
                    if (existingUser != null)
                        throw new InvalidOperationException("Пользователь с таким email уже существует");
                }

                if (user.UserName != updateDto.UserName)
                {
                    var existingUsername = await _userRepository.GetByUsernameAsync(updateDto.UserName);
                    if (existingUsername != null)
                        throw new InvalidOperationException("Пользователь с таким именем уже существует");
                }

                user.UserName = updateDto.UserName;
                user.Email = updateDto.Email;

                if (!string.IsNullOrEmpty(updateDto.Level))
                {
                    var validLevels = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };
                    if (!validLevels.Contains(updateDto.Level))
                        throw new ArgumentException("Недопустимый уровень");
                    user.Level = updateDto.Level;
                }

                if (!string.IsNullOrEmpty(updateDto.NewPassword))
                {
                    if (string.IsNullOrEmpty(updateDto.CurrentPassword))
                        throw new UnauthorizedAccessException("Для смены пароля введите текущий пароль");

                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(updateDto.CurrentPassword, user.PasswordHash);
                    if (!isValidPassword)
                        throw new UnauthorizedAccessException("Неверный текущий пароль");

                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
                }

                _userRepository.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Профиль пользователя обновлен: {UserId}", userId);
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении профиля пользователя: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserStatsDto> GetUserStatsAsync(long userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("Пользователь не найден");

                var learningWords = await _context.UsersWords
                     .CountAsync(uw => uw.UserId == userId && uw.Status == "learning");

                var learnedWords = await _context.UsersWords
                   .CountAsync(uw => uw.UserId == userId && uw.Status == "learned");

                var completedTests = await _context.TestResults
                   .CountAsync(tr => tr.UserId == userId);

                var completedLessons = await _context.UserGrammarProgress
                    .CountAsync(ugp => ugp.UserId == userId && ugp.Completed);

                var completedVocab = await _context.UserVocabProgresses
                    .CountAsync(uvp => uvp.UserId == userId && uvp.Completed);

                return new UserStatsDto
                {
                    LearningWords = learningWords,
                    LearnedWords = learnedWords,
                    CompletedTests = completedTests,
                    CompletedLessons = completedLessons + completedVocab,
                    MemberSince = user.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики пользователя: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> VerifyPasswordAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}