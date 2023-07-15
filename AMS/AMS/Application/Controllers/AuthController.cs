using AMS.Application.Helpers;
using AMS.Application.Interfaces;
using AMS.Domain.Models;
using AMS.Web.ViewModels.Requests;
using AMS.Web.ViewModels.Responses;
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
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly AMS.Config.AppSetings _appSettings;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration,
            IOptions<AMS.Config.AppSetings> appSettings, ILogger<AuthController> logger)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorApiResponse { Message = "Invalid request data" });

            var registerResult = _userService.RegisterAsync(request).Result;

            if (!registerResult.Success)
                return BadRequest(new ErrorApiResponse { Message = registerResult.Message });

            return Ok(new ApiResponse<string> { Success = true, Message = "Registration successful" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorApiResponse { Message = "Invalid request data" });

            var loginResult = _userService.LoginAsync(request).Result;

            if (!loginResult.Success)
                return Unauthorized();

            var token = JwtTokenGenerator.GenerateJwtToken(_appSettings.JwtSecret, _appSettings.JwtIssuer, _appSettings.JwtAudience,
                loginResult.UserId.ToString(), loginResult.UserName);

            return Ok(new ApiResponse<string> { Success = true, Data = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var session = HttpContext.Session;

            session.Clear();

            await HttpContext.SignOutAsync();

            return Ok(new ApiResponse<string> { Success = true, Message = "User logged out successfully" });
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Auth controller accessed");
            return Ok();
        }

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
