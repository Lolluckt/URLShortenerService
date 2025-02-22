using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UrlService.Application.DTOs;
using UrlService.Application.Interfaces;

namespace UrlService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var about = await _aboutService.GetAboutAsync();
                return Ok(about);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] AboutDto dto)
        {
            try
            {
                int adminUserId = GetUserIdFromToken(User);
                await _aboutService.UpdateAboutAsync(dto, adminUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var subClaim = user.FindFirst(JwtRegisteredClaimNames.Sub);
            if (subClaim == null)
                throw new Exception("No 'sub' claim in token.");

            return int.Parse(subClaim.Value);
        }
    }
}
