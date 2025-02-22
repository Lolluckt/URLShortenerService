using Microsoft.EntityFrameworkCore;
using UrlService.Domain.Entities;
using UrlService.Infrastructure.Data;
using UrlService.Infrastructure.Interfaces;


namespace UrlService.Infrastructure.Repositories
{
    public class AboutRepository : GenericRepository<About>, IAboutRepository
    {
        public AboutRepository(UrlServiceDbContext context) : base(context)
        {
        }

        public async Task<About> GetSingleAsync()
        {
            return await _context.Abouts.FirstOrDefaultAsync();
        }
    }
}
