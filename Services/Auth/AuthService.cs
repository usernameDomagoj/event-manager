using EventManager.Data;
using EventManager.Entities;
using EventManager.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using EventManager.Enums;

namespace EventManager.Services.Auth
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        private string GenerateRefreshToken()
        {
            var RandomNumber = new byte[32];
            using var Rng = RandomNumberGenerator.Create();
            Rng.GetBytes(RandomNumber);

            return Convert.ToBase64String(RandomNumber);
        }

        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var User = await context.Users.FindAsync(userId);
            if (User is null || User.RefreshToken != refreshToken || User.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return User;
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var RefreshToken = GenerateRefreshToken();
            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();

            return RefreshToken;
        }

        private string CreateToken(User user)
        {
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, nameof(user.Role))
            };

            var Key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512);

            var TokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: Claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: Creds
            );

            return new JwtSecurityTokenHandler().WriteToken(TokenDescriptor);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User? user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user!),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user!)
            };
        }

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {
            var User = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (User is null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(User, User.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateTokenResponse(User);
        }

        public async Task<User?> RegisterAsync(UserRegisterDto request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return null;
            }

            var User = new User();
            var HashedPassword = new PasswordHasher<User>()
                .HashPassword(User, request.Password);

            User.Name = request.Name;
            User.Username = request.Username;
            User.Email = request.Email;
            User.Status = UserStatus.Pending;  // TODO make endpoint for approval and setting the role (only by admin)
            User.Role = UserRole.User;  // TODO make endpoint for approval and setting the role (only by admin)
            User.PasswordHash = HashedPassword;
            User.CreatedDate = DateTime.UtcNow;

            context.Users.Add(User);
            await context.SaveChangesAsync();

            return User;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var User = await ValidateRefreshTokenAsync(request.Id, request.RefreshToken);
            if (User is null)
            {
                return null;
            }

            return await CreateTokenResponse(User);
        }
    }
}
