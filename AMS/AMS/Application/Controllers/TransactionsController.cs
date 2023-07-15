using AMS.Application.Exceptions;
using AMS.Application.Interfaces;
using AMS.Web.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Application.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult GetTransaction(int id)
        {
            var transaction = _transactionService.GetTransactionById(id);

            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost]
        public IActionResult CreateTransaction(TransactionRequest request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request data.");

            try
            {
                _transactionService.CreateTransaction(request.AccountId, request.Amount, request.Type);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Transactions controller accessed");
            return Ok();
        }
    }
}
