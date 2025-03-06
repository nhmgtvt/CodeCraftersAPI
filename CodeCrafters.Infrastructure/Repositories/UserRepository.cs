using CodeCrafters.Domain.Entities.Auth;
using CodeCrafters.Domain.Interfaces.Auth;
using CodeCrafters.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeCrafters.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByProviderAsync(string provider, string providerUserId)
        {
            return await _context.ExternalLogins
            .Where(el => el.Provider == provider && el.ProviderUserId == providerUserId)
            .Select(el => el.User) // Fetch the linked user
            .FirstOrDefaultAsync();
        }

        public async Task LinkExternalLoginAsync(ExternalLogin externalLogin)
        {
            await _context.ExternalLogins.AddAsync(externalLogin);
            await _context.SaveChangesAsync();
        }
    }
}
