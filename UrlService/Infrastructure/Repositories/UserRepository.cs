using Microsoft.EntityFrameworkCore;
using UrlService.Domain.Entities;
using UrlService.Infrastructure.Data;
using UrlService.Infrastructure.Interfaces;

namespace UrlService.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(UrlServiceDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}

