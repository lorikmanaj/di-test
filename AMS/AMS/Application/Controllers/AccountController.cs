using AMS.Application.Accounts.Commands;
using AMS.Application.Handlers;
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
        public IActionResult Register(RegisterCommand command)
        {
            var result = _registerCommandHandler.Handle(command);

            if (result.Result.Success)
                return Ok("Registration successful");

            return BadRequest(result.Result.Message);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginCommand command)
        {
            var result = _loginCommandHandler.Handle(command);

            if (result.Result.Success)
                return Ok(result.Result.Token);

            return BadRequest(result.Result.Message);
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

            return Ok("Logout successful");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Account page accessed");
            return Ok();
        }
    }
}
