using UrlService.Domain.Entities;

namespace UrlService.Infrastructure.Interfaces
{
    public interface IUrlRepository : IGenericRepository<Url>
    {
        Task<Url> GetByShortUrlAsync(string shortUrl);

        Task<bool> ExistsByOriginalUrlAsync(string originalUrl);
    }
}
