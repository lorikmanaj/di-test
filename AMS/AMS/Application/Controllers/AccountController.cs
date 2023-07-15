using AMS.Application.Handlers;
using AMS.Web.ViewModels.Accounts.Commands;
using AMS.Web.ViewModels.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMS.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IRegisterCommandHandler _registerCommandHandler;
        private readonly ILoginCommandHandler _loginCommandHandler;

        public AccountController(
            ILogger<AccountController> logger,
            IRegisterCommandHandler registerCommandHandler,
            ILoginCommandHandler loginCommandHandler)
        {
            _logger = logger;
            _registerCommandHandler = registerCommandHandler;
            _loginCommandHandler = loginCommandHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var result = await _registerCommandHandler.Handle(command);

            if (result.Success)
                return Ok(new ApiResponse<string> { Success = true, Message = "Registration successful" });

            return BadRequest(new ErrorApiResponse { Message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _loginCommandHandler.Handle(command);

            if (result.Success)
                return Ok(new ApiResponse<string> { Success = true, Data = result.Token });

            return BadRequest(new ErrorApiResponse { Message = result.Message });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdentity = User.Identity as ClaimsIdentity;

            if (userIdentity != null)
            {
                var jwtToken = userIdentity.FindFirst("jwtToken")?.Value;

                await HttpContext.SignOutAsync();

                HttpContext.Session.Clear();
            }

            return Ok(new ApiResponse<string> { Success = true, Message = "Logout successful" });
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Account page accessed");
            return Ok(new ApiResponse<string> { Success = true });
        }
    }

}
