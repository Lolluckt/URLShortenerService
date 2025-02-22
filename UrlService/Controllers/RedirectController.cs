using Microsoft.AspNetCore.Mvc;
using UrlService.Application.Interfaces;

namespace UrlService.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IUrlService _urlService;

        public RedirectController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string shortUrl)
        {
            try
            {
                var originalUrl = await _urlService.GetOriginalUrlByShortAsync(shortUrl);
                if (string.IsNullOrEmpty(originalUrl))
                {
                    return NotFound("No link found for this short slug.");
                }

                return Redirect(originalUrl);
            }
            catch
            {
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}
