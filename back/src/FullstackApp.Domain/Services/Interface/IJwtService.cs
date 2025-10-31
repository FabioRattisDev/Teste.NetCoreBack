using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullstackApp.Domain.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, string email);
    }
}
