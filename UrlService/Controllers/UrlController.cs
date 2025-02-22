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
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var urls = await _urlService.GetAllUrlsAsync();
                return Ok(urls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var details = await _urlService.GetUrlDetailsAsync(id);
                return Ok(details);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { message = ex.Message });
                }

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateUrlDto dto)
        {
            try
            {
                int currentUserId = GetUserIdFromToken(User);
                var created = await _urlService.CreateUrlAsync(dto, currentUserId);
                return Ok(created);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Conflict(new { message = ex.Message });
                }
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int currentUserId = GetUserIdFromToken(User);
                bool isAdmin = User.IsInRole("Admin");

                await _urlService.DeleteUrlAsync(id, currentUserId, isAdmin);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("permission"))
                {
                    return Forbid(ex.Message);
                }
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { message = ex.Message });
                }

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

