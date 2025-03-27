namespace EventManager.Models.User
{
    public class UserLoginDto
    {
        public required string Username { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }
}
