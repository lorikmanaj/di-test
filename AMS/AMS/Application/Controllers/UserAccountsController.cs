using AMS.Application.Exceptions;
using AMS.Application.Interfaces;
using AMS.Domain.Models;
using AMS.Web.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Application.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class UserAccountsController : ControllerBase
    {
        private readonly ILogger<UserAccountsController> _logger;
        private readonly IAccountService _accountService;

        public UserAccountsController(IAccountService accountService, ILogger<UserAccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAccounts()
        {
            IEnumerable<Account> accounts = _accountService.GetAccounts();
            return Ok(new ApiResponse<IEnumerable<Account>> { Data = accounts });
        }

        [HttpGet("{id}")]
        public IActionResult GetAccount(int id)
        {
            Account account = _accountService.GetAccountById(id);
            if (account == null)
                return NotFound(new ErrorApiResponse { Message = "Account not found" });

            return Ok(new ApiResponse<Account> { Data = account });
        }

        [HttpPost]
        public IActionResult CreateAccount(Account account)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            try
            {
                _accountService.CreateAccount(account.Name, account.Type, account.Balance);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorApiResponse { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, Account account)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            if (id != account.Id)
                return BadRequest(new ErrorApiResponse { Message = "Account ID mismatch" });

            try
            {
                _accountService.UpdateAccount(account);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorApiResponse { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            Account account = _accountService.GetAccountById(id);
            if (account == null)
                return NotFound(new ErrorApiResponse { Message = "Account not found" });

            _accountService.DeleteAccount(id);
            return Ok();
        }

        public IActionResult Index()
        {
            _logger.LogInformation("UserAccounts controller accessed");
            return Ok();
        }
    }
}
