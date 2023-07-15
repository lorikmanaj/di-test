using AMS.Application.Exceptions;
using AMS.Application.Helpers;
using AMS.Application.Interfaces;
using AMS.Domain.Models;
using AMS.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AMS.Config.AppSetings _appSettings;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration, IOptions<AMS.Config.AppSetings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            var registerResult = _userService.RegisterAsync(request);

            if (!registerResult.Result.Success)
                return BadRequest(registerResult.Result);

            return Ok(registerResult.Result.Message);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            var loginResult = _userService.LoginAsync(request).Result;

            if (!loginResult.Success)
                return Unauthorized();

            var token = JwtTokenGenerator.GenerateJwtToken(_appSettings.JwtSecret, _appSettings.JwtIssuer, _appSettings.JwtAudience,
                                                            loginResult.UserId.ToString(), loginResult.UserName);

            return Ok(new { Token = token });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var session = HttpContext.Session;

            session.Clear();

            await HttpContext.SignOutAsync();

            return Ok(new { Message = "User logged out successfully" });
        }


        // Helper method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    // Add additional claims as needed
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
