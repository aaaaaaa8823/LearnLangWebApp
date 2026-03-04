using LearnEnglishWebApp.Data;
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
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly JWTSettings _jwtSettings;

        public UserService(AppDbContext context, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;  
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Level = user.Level,
                CreatedAt = user.CreatedAt
            };

        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Level = user.Level,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null)
                throw new Exception("Пользователь уже существует");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Level = "A1",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await CreateDefaultCollectionsAsync(user.Id);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Level = user.Level,
                CreatedAt = user.CreatedAt
            };
        }

        private async Task CreateDefaultCollectionsAsync(long userId)
        {
            var collections = new[]
            {
                new Collection { UserId = userId, Name = "В процессе", IsDefault = true },
                new Collection { UserId = userId, Name = "Выученные", IsDefault = true },
            };

            await _context.Collections.AddRangeAsync(collections);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null) 
                throw new Exception("Пользователь не найден");

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isValidPassword)
                throw new Exception("Неверный пароль");

            //генерировать JWT токен
            var token = GenerateJWTToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Level = user.Level,
                    CreatedAt = user.CreatedAt
                },
                ResponseLenght = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours)
              
            };
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
            var user = await _context.Users.FindAsync(userId);


            if (user == null)
                throw new Exception("Пользователь не найден");

            var validLevels = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };

            if (!validLevels.Contains(newLevel))
                throw new Exception("Недопустимый уровень");

            user.Level = newLevel;
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Level = user.Level,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
