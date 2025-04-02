using EventManager.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventManager.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        

        // relation many to many
        [JsonIgnore]
        public List<Event>? Events { get; set; }
    }
}
