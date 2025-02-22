using UrlService.Application.DTOs;

namespace UrlService.Application.Interfaces
{

    public interface IAboutService
    {
        Task<AboutDto> GetAboutAsync();

        Task UpdateAboutAsync(AboutDto aboutDto, int adminUserId);
    }
}
