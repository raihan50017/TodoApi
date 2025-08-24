using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Entities;
using TodoApi.Repositories;
using TodoApi.Services;
using BCrypt.Net;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;

        public AuthController(IUnitOfWork uow, IJwtService jwtService)
        {
            _uow = uow;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var existing = (await _uow.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (existing != null)
                return Conflict(new { Error = "EmailAlreadyExists", Message = "A user with this email already exists." });

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(nameof(Signup), new { id = user.Id }, new { user.Id, user.Email });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = (await _uow.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { Error = "InvalidCredentials", Message = "Email or password is incorrect." });

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = user.RefreshTokenExpiresAtUtc
            });

            return Ok(new AuthResponse
            {
                Token = accessToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15) 
            });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return Unauthorized(new { Error = "NoRefreshToken", Message = "Refresh token is missing." });
            }

            var user = (await _uow.Users.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault();

            if (user == null || user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
            {
                return Unauthorized(new { Error = "InvalidRefreshToken", Message = "Refresh token is invalid or expired." });
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

            _uow.Users.Update(user);
            await _uow.SaveChangesAsync();

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = user.RefreshTokenExpiresAtUtc
            });

            return Ok(new AuthResponse
            {
                Token = newAccessToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15)
            });
        }
    }
}
