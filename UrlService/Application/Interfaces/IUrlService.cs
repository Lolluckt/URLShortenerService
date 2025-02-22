using UrlService.Application.DTOs;

namespace UrlService.Application.Interfaces
{
    public interface IUrlService
    {
        Task<UrlDto> CreateUrlAsync(CreateUrlDto dto, int creatorUserId);

        Task<List<UrlDto>> GetAllUrlsAsync();

        Task<UrlDetailsDto> GetUrlDetailsAsync(int urlId);

        Task DeleteUrlAsync(int urlId, int currentUserId, bool isAdmin);

        Task<string> GetOriginalUrlByShortAsync(string shortUrl);
    }
}
