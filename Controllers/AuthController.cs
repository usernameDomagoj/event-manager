using EventManager.Data;
using EventManager.Entities;
using EventManager.Enums;
using EventManager.Models.User;
using EventManager.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register(UserRegisterDto request)
        {
            var User = await authService.RegisterAsync(request);

            if (User is null)
            {
                return BadRequest("Username already exists.");
            }

            return Ok(User);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Login(UserLoginDto request)
        {
            var Result = await authService.LoginAsync(request);

            if (Result is null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(Result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(result);
        }

        [HttpPost("update-user")]
        [AuthorizeRoles(UserRole.Admin)]
        public async Task<ActionResult> UpdateUser(UpdateUserDto request)
        {
            var User = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
    
            if (User is null)
            {
                return BadRequest($"User with username {request.Username} does not exist.");
            }

            if (Enum.TryParse(request.Role.ToString(), out UserRole Role))
            {
                User.Role = (UserRole)Enum.Parse(typeof(UserRole), request.Role.ToString());
            }

            if (Enum.TryParse(request.Status.ToString(), out UserStatus Status))
            {
                User.Status = (UserStatus)Enum.Parse(typeof(UserStatus), request.Status.ToString());
            }

            if (!Enum.IsDefined(typeof(UserRole), User.Role) | !Enum.IsDefined(typeof(UserStatus), User.Status))
            {
               
                return BadRequest("User status or role is invalid.");
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
