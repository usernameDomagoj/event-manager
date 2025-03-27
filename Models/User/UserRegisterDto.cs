﻿namespace EventManager.Models.User
{
    public class UserRegisterDto
    {
        public required string Name { get; set; } = string.Empty;
        public required string Username { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
    }
}
