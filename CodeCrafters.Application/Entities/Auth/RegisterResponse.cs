namespace CodeCrafters.Application.Entities.Auth
{
    public class RegisterResponse
    {
        public required string Token { get; set; }
        public required UserDto User { get; set; }
    }
}
