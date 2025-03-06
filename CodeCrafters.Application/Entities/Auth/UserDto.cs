using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCrafters.Application.Entities.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
    }
}
