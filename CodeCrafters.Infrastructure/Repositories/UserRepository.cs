using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task<User?> GetUserByProviderAsync(string provider, string providerUserId)
        {
            throw new NotImplementedException();
        }

        public Task LinkExternalLoginAsync(ExternalLogin externalLogin)
        {
            throw new NotImplementedException();
        }
    }
}
