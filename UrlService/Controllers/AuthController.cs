using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlService.Application.DTOs;
using UrlService.Application.Interfaces;
using UrlService.Domain.Roles;

namespace UrlService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var userDto = await _authService.RegisterAsync(registerDto);
                return Ok(userDto);
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var userDto = await _authService.LoginAsync(loginDto);

                var token = GenerateJwtToken(userDto);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(2)
                };
                Response.Cookies.Append("AccessToken", token, cookieOptions);

                var roleString = (userDto.Role == Role.Admin) ? "Admin" : "User";

                var userObject = new
                {
                    userDto.Id,
                    userDto.FirstName,
                    userDto.LastName,
                    userDto.Email,
                    Role = roleString
                };

                return Ok(new
                {
                    User = userObject,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { message = ex.Message });
                }
                else if (ex.Message.Contains("Invalid password"))
                {
                    return BadRequest(new { message = ex.Message });
                }
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private string GenerateJwtToken(UserDto user)
        {
            var secretKey = _configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("JWT Secret not configured.");

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            var roleString = (user.Role == Role.Admin) ? "Admin" : "User";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, roleString)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
