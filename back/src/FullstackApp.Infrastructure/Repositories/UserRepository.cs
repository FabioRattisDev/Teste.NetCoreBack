using FullstackApp.Domain.Entities;
using FullstackApp.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullstackApp.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

        public async Task<bool> EmailExistsAsync(string email)
            => await _dbSet.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }
}
