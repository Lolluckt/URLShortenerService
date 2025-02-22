using UrlService.Domain.Entities;

namespace UrlService.Infrastructure.Interfaces
{
    public interface IAboutRepository : IGenericRepository<About>
    {
        Task<About> GetSingleAsync();
    }
}
