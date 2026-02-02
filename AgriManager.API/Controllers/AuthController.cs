using AgriManager.API.DTOs;
using AgriManager.API.Models;
using AgriManager.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgriManager.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AgriManagerDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            AgriManagerDbContext context,
            IConfiguration configuration
        )
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.CustomerId == request.CustomerId &&
                u.UserName == request.UserName &&
                u.Password == request.Password && // (hash later)
                u.IsActive
            );

            if (user == null)
            {
                return Unauthorized(new ApiResponseDto<LoginResponseDto>
                {
                    Status = false,
                    Message = "Invalid CustomerId, UserName, or Password",
                    Data = null
                });
            }

            // ✅ Create JWT Claims (NO PASSWORD)
            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim("CustomerId", user.CustomerId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new ApiResponseDto<LoginResponseDto>
            {
                Status = true,
                Message = "Login successful",
                Data = new LoginResponseDto
                {
                    UserId = user.UserId,
                    CustomerId = user.CustomerId,
                    UserName = user.UserName,
                    Token = jwtToken
                }
            });
        }
    }
}
