using System.ComponentModel.DataAnnotations;

namespace EventManager.Models.User
{
    public class UserLoginDto
    {
        [StringLength(50, MinimumLength = 1)]
        public required string Username { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }
}
