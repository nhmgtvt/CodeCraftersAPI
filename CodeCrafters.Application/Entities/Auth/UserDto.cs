namespace CodeCrafters.Application.Entities.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
    }
}
