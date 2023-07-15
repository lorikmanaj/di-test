using AMS.Application.Exceptions;
using AMS.Application.Interfaces;
using AMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Application.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            var result = await _userService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("{userId}")]
        public IActionResult GetUser(int userId)
        {
            var user = _userService.GetUserById(userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return Ok(user);
        }
    }

}
