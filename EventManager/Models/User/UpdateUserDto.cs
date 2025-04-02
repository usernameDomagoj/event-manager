using EventManager.Enums;
using System.ComponentModel.DataAnnotations;

namespace EventManager.Models.User
{
    public class UpdateUserDto
    {
        [StringLength(50, MinimumLength = 1)]
        public required string Username { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
    }
}
