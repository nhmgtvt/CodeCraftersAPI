using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCrafters.Domain.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        bool VerifyPassword(string password, string hashedPassword);
        string HashPassword(string password);
    }
}
