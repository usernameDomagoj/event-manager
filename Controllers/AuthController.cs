using EventManager.Entities;
using EventManager.Models.User;
using EventManager.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request)
        {
            var User = await authService.RegisterAsync(request);

            if (User is null)
            {
                return BadRequest("Username already exists.");
            }

            return Ok(User);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserLoginDto request)
        {
            var Result = await authService.LoginAsync(request);

            if (Result is null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(Result);
        }
    }
}
