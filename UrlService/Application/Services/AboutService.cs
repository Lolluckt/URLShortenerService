using UrlService.Application.DTOs;
using UrlService.Application.Interfaces;
using UrlService.Domain.Roles;
using UrlService.Infrastructure.Interfaces;

namespace UrlService.Application.Services
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _aboutRepository;
        private readonly IUserRepository _userRepository;

        public AboutService(IAboutRepository aboutRepository, IUserRepository userRepository)
        {
            _aboutRepository = aboutRepository;
            _userRepository = userRepository;
        }

        public async Task<AboutDto> GetAboutAsync()
        {
            var entity = await _aboutRepository.GetSingleAsync();
            if (entity == null)
            {
                return new AboutDto
                {
                    Id = 0,
                    Content = "No About info yet.",
                    ModifiedDate = null
                };
            }

            return new AboutDto
            {
                Id = entity.Id,
                Content = entity.Content,
                ModifiedDate = entity.ModifiedDate
            };
        }

        public async Task UpdateAboutAsync(AboutDto aboutDto, int adminUserId)
        {
            var adminUser = await _userRepository.GetByIdAsync(adminUserId);
            if (adminUser == null || adminUser.Role != Role.Admin)
            {
                throw new Exception("You do not have permissions to update About.");
            }
            var entity = await _aboutRepository.GetSingleAsync();
            if (entity == null)
            {
                throw new Exception("No About record found in the database. Update operation cannot be performed.");
            }

            entity.Content = aboutDto.Content;
            entity.ModifiedDate = DateTime.UtcNow;

            await _aboutRepository.UpdateAsync(entity);
        }
    }
}
