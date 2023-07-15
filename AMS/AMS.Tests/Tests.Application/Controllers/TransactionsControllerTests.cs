using AMS.Application.Controllers;
using AMS.Application.Exceptions;
using AMS.Application.Interfaces;
using AMS.Domain.Models;
using AMS.Web.ViewModels.Requests;
using AMS.Web.ViewModels.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AMS.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly TransactionsController _controller;
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<ILogger<TransactionsController>> _loggerMock;

        public TransactionsControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _loggerMock = new Mock<ILogger<TransactionsController>>();

            _controller = new TransactionsController(_transactionServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetTransaction_ValidId_ReturnsOk()
        {
            int transactionId = 1;
            var transaction = new Transaction { Id = transactionId, Amount = 100, Type = TransactionType.In };

            _transactionServiceMock.Setup(x => x.GetTransactionById(transactionId)).Returns(transaction);

            var result = _controller.GetTransaction(transactionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Transaction>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(transaction, apiResponse.Data);
        }

        [Fact]
        public void GetTransaction_InvalidId_ReturnsNotFound()
        {
            int transactionId = 1;

            _transactionServiceMock.Setup(x => x.GetTransactionById(transactionId)).Returns((Transaction)null);

            var result = _controller.GetTransaction(transactionId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorApiResponse>(notFoundResult.Value);
            Assert.Equal("Transaction not found", errorResponse.Message);
        }

        [Fact]
        public void CreateTransaction_ValidRequest_ReturnsOk()
        {
            var request = new TransactionRequest { AccountId = 1, Amount = 100, Type = TransactionType.In };

            var result = _controller.CreateTransaction(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Transaction created successfully", apiResponse.Message);
        }

        [Fact]
        public void CreateTransaction_InvalidRequest_ThrowsValidationException()
        {
            var request = new TransactionRequest { AccountId = 1, Amount = -100, Type = TransactionType.Out };
            _controller.ModelState.AddModelError("Amount", "Invalid amount");

            Assert.Throws<ValidationException>(() => _controller.CreateTransaction(request));
        }

        [Fact]
        public void CreateTransaction_ArgumentException_ReturnsBadRequest()
        {
            var request = new TransactionRequest { AccountId = 1, Amount = 100, Type = TransactionType.Out };
            var errorMessage = "Invalid account ID";
            _transactionServiceMock.Setup(x => x.CreateTransaction(request.AccountId, request.Amount, request.Type)).Throws(new ArgumentException(errorMessage));

            var result = _controller.CreateTransaction(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorApiResponse>(badRequestResult.Value);
            Assert.Equal(errorMessage, errorResponse.Message);
        }

        [Fact]
        public void CreateTransaction_InvalidOperationException_ReturnsConflict()
        {
            var request = new TransactionRequest { AccountId = 1, Amount = 100, Type = TransactionType.Out };
            var errorMessage = "Insufficient balance";

            _transactionServiceMock.Setup(x => x.CreateTransaction(request.AccountId, request.Amount, request.Type)).Throws(new InvalidOperationException(errorMessage));

            var result = _controller.CreateTransaction(request);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorApiResponse>(conflictResult.Value);
            Assert.Equal(errorMessage, errorResponse.Message);
        }

        [Fact]
        public void Index_ReturnsOk()
        {
            var result = _controller.Index();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }
    }
}
