using UrlService.Application.DTOs;
using UrlService.Application.Interfaces;
using UrlService.Domain.Entities;
using UrlService.Infrastructure.Interfaces;

namespace UrlService.Application.Services
{
    public class UrlShortenerService : IUrlService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly IUserRepository _userRepository; 
        public UrlShortenerService(IUrlRepository urlRepository, IUserRepository userRepository)
        {
            _urlRepository = urlRepository;
            _userRepository = userRepository;
        }

        public async Task<UrlDto> CreateUrlAsync(CreateUrlDto dto, int creatorUserId)
        {
            var encryptedUrl = EncryptionHelper.Encrypt(dto.OriginalUrl);

            var exists = await _urlRepository.ExistsByOriginalUrlAsync(encryptedUrl);
            if (exists)
            {
                throw new Exception("Such a URL already exists!");
            }

            var shortUrl = await GenerateUniqueShortUrl();

            var newUrl = new Url
            {
                OriginalUrl = encryptedUrl,
                ShortUrl = shortUrl,
                CreatedByUserId = creatorUserId,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };
            await _urlRepository.AddAsync(newUrl);

            var creator = await _userRepository.GetByIdAsync(creatorUserId);

            return new UrlDto
            {
                Id = newUrl.Id,
                OriginalUrl = dto.OriginalUrl,
                ShortUrl = newUrl.ShortUrl,
                CreatedDate = newUrl.CreatedDate,
                CreatedByEmail = creator?.Email
            };
        }

        public async Task<List<UrlDto>> GetAllUrlsAsync()
        {
            var allUrls = await _urlRepository.GetAllAsync();
            var activeUrls = allUrls.Where(u => !u.IsDeleted).ToList();

            var result = new List<UrlDto>();
            foreach (var url in activeUrls)
            {
                var decrypted = EncryptionHelper.Decrypt(url.OriginalUrl);
                var creator = await _userRepository.GetByIdAsync(url.CreatedByUserId);

                result.Add(new UrlDto
                {
                    Id = url.Id,
                    OriginalUrl = decrypted,
                    ShortUrl = url.ShortUrl,
                    CreatedDate = url.CreatedDate,
                    CreatedByEmail = creator?.Email
                });
            }
            return result;
        }

        public async Task<UrlDetailsDto> GetUrlDetailsAsync(int urlId)
        {
            var url = await _urlRepository.GetByIdAsync(urlId);
            if (url == null || url.IsDeleted)
            {
                throw new Exception("URL not found or deleted.");
            }

            var creator = await _userRepository.GetByIdAsync(url.CreatedByUserId);
            var decrypted = EncryptionHelper.Decrypt(url.OriginalUrl);

            return new UrlDetailsDto
            {
                Id = url.Id,
                OriginalUrl = decrypted,
                ShortUrl = url.ShortUrl,
                CreatedByUserId = url.CreatedByUserId,
                CreatedByEmail = creator?.Email,
                CreatedDate = url.CreatedDate,
                UpdatedDate = url.UpdatedDate,
                IsDeleted = url.IsDeleted
            };
        }

        public async Task DeleteUrlAsync(int urlId, int currentUserId, bool isAdmin)
        {
            var url = await _urlRepository.GetByIdAsync(urlId);
            if (url == null)
                return;

            if (!isAdmin && url.CreatedByUserId != currentUserId)
            {
                throw new Exception("You do not have permission to delete this URL.");
            }

            url.IsDeleted = true;
            await _urlRepository.UpdateAsync(url);
        }

        private async Task<string> GenerateUniqueShortUrl()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            while (true)
            {
                var slug = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var existing = await _urlRepository.GetByShortUrlAsync(slug);
                if (existing == null)
                {
                    return slug;
                }
            }
        }
        public async Task<string> GetOriginalUrlByShortAsync(string shortUrl)
        {
            var entity = await _urlRepository.GetByShortUrlAsync(shortUrl);
            if (entity == null || entity.IsDeleted)
                return null;

            var decrypted = EncryptionHelper.Decrypt(entity.OriginalUrl);
            return decrypted;
        }

    }
}