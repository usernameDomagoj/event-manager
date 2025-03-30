using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Models.User
{
    public class UserRegisterDto
    {
        [StringLength(50, MinimumLength = 1)]
        public required string Name { get; set; } = string.Empty;
        [StringLength(50, MinimumLength = 1)]
        public required string Username { get; set; } = string.Empty;
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public required string Password { get; set; } = string.Empty;
        [DisplayName ("Password Confirm")]
        [Compare(nameof(Password))]
        public required string PasswordConfirm {  get; set; } = string.Empty;
    }
}
