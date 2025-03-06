using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCrafters.Application.Entities.Auth
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required UserDto User { get; set; }
    }
}
