using Microsoft.EntityFrameworkCore;
using UrlService.Domain.Entities;
using UrlService.Infrastructure.Data;
using UrlService.Infrastructure.Interfaces;


namespace UrlService.Infrastructure.Repositories
{
    public class UrlRepository : GenericRepository<Url>, IUrlRepository
    {
        public UrlRepository(UrlServiceDbContext context) : base(context)
        {
        }

        public async Task<Url> GetByShortUrlAsync(string shortUrl)
        {
            return await _context.Urls
                .FirstOrDefaultAsync(u => u.ShortUrl == shortUrl && !u.IsDeleted);
        }

        public async Task<bool> ExistsByOriginalUrlAsync(string originalUrl)
        {
            return await _context.Urls
                .AnyAsync(u => u.OriginalUrl == originalUrl && !u.IsDeleted);
        }
    }
}
