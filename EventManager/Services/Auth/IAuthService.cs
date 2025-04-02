using EventManager.Entities;
using EventManager.Models.User;

namespace EventManager.Services.Auth
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
