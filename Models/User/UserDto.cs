using EventManager.Models.Event;
using System.Text.Json.Serialization;

namespace EventManager.Models.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
