﻿namespace CodeCrafters.Application.Entities.Auth
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required UserDto User { get; set; }
    }
}
