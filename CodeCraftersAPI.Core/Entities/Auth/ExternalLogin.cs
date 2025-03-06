namespace CodeCrafters.Domain.Entities.Auth
{
    public class ExternalLogin
    {
        public Guid ExternalLoginId { get; set; }
        public required string Provider { get; set; }
        public required string ProviderUserId { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
