using AMS.Application.Exceptions;
using AMS.Application.Interfaces;
using AMS.Domain.Models;
using AMS.Web.ViewModels.Requests;
using AMS.Web.ViewModels.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Application.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            var result = await _userService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(new ErrorApiResponse { Message = result.Message });

            return Ok(new ApiResponse<string> { Message = result.Message });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _userService.GetUserById(userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return Ok(new ApiResponse<User> { Data = user });
        }

        public IActionResult Index()
        {
            _logger.LogInformation("User controller accessed");
            return Ok();
        }
    }
}
