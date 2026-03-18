using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Implementations;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Classes;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context; //для прямых операций с коллекциями
        private readonly JWTSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, AppDbContext context, IOptions<JWTSettings> jwtSettings, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _jwtSettings = jwtSettings.Value;
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
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    return null;
                }
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя по ID: {UserId}", id);
                throw;
            }
        }
        //var user = await _context.Users.FindAsync(id);

        //if (user == null) return null;

        //return new UserDto
        //{
        //    Id = user.Id,
        //    UserName = user.UserName,
        //    Email = user.Email,
        //    Level = user.Level,
        //    CreatedAt = user.CreatedAt
        //};

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return null;
                }
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получаени юзера по email");
                throw;
            }
        }
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null) return null;

        //    return new UserDto
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        Level = user.Level,
        //        CreatedAt = user.CreatedAt
        //    };
        //}

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
            //var existingUser = await _context.Users
            //    .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            //if (existingUser != null)
            //    throw new Exception("Пользователь уже существует");

            //var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            //var user = new User
            //{
            //    UserName = registerDto.UserName,
            //    Email = registerDto.Email,
            //    PasswordHash = passwordHash,
            //    Level = "A1",
            //    CreatedAt = DateTime.UtcNow
            //};

            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            //await CreateDefaultCollectionsAsync(user.Id);

            //return new UserDto
            //{
            //    Id = user.Id,
            //    UserName = user.UserName,
            //    Email = user.Email,
            //    Level = user.Level,
            //    CreatedAt = user.CreatedAt
            //};
        }

        private async Task CreateDefaultCollectionsAsync(long userId)
        {
            var collections = new[]
            {
                new Collection {
                    UserId = userId,
                    Name = "В процессе",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow 
                },
                new Collection {
                    UserId = userId,
                    Name = "Выученные",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow 
                },
            };

            await _context.Collections.AddRangeAsync(collections);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);

                if (user == null)
                    throw new UnauthorizedAccessException("Неверный email или пароль");

                bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

                if (!isValidPassword)
                    throw new UnauthorizedAccessException("Неверный email или пароль");

                var token = GenerateJWTToken(user);

                _logger.LogInformation("Пользователь успешно вошел: {Email}", user.Email);

                return new AuthResponseDto
                {
                    Token = token,
                    User = MapToDto(user),
                    ResponseLenght = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя: {Email}", loginDto.Email);
                throw;
            }
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            //if (user == null) 
            //    throw new Exception("Пользователь не найден");

            //bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            //if (!isValidPassword)
            //    throw new Exception("Неверный пароль");

            ////генерировать JWT токен
            //var token = GenerateJWTToken(user);

            //return new AuthResponseDto
            //{
            //    Token = token,
            //    User = new UserDto
            //    {
            //        Id = user.Id,
            //        UserName = user.UserName,
            //        Email = user.Email,
            //        Level = user.Level,
            //        CreatedAt = user.CreatedAt
            //    },
            //    ResponseLenght = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours)

            //};
        }

        private string GenerateJWTToken(User user)
        {
            if (_jwtSettings == null)
            {
                throw new InvalidOperationException("JWT Settings не настроен. Чек appsetting");
            }

            if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
                throw new Exception("JWT SecretKey не настроен");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", user.UserName),
            new Claim("level", user.Level)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
                    throw new ArgumentException("Недопустимый уровень. Используйте A1, A2, B1, B2, C1, C2");

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
    }
}