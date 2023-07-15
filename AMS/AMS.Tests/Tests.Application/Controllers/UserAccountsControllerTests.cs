using AMS.Application.Controllers;
using AMS.Application.Interfaces;
using AMS.Domain.Models;
using AMS.Web.ViewModels.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AMS.Tests.Controllers
{
    public class UserAccountsControllerTests
    {
        private readonly UserAccountsController _controller;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<ILogger<UserAccountsController>> _loggerMock;

        public UserAccountsControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _loggerMock = new Mock<ILogger<UserAccountsController>>();

            _controller = new UserAccountsController(
                _accountServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void GetAccounts_ReturnsOkWithAccounts()
        {
            var accounts = new List<Account>
            {
                new Account { Id = 1, Name = "Account 1", Type = AccountType.Business, Balance = 100 },
                new Account { Id = 2, Name = "Account 2", Type = AccountType.Debit, Balance = 200 }
            };

            _accountServiceMock.Setup(x => x.GetAccounts()).Returns(accounts);

            var result = _controller.GetAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<Account>>>(okResult.Value);
            Assert.Equal(accounts, apiResponse.Data);
        }

        [Fact]
        public void GetAccount_ExistingId_ReturnsOkWithAccount()
        {
            var account = new Account { Id = 1, Name = "Account 1", Type = AccountType.Premium, Balance = 100 };

            _accountServiceMock.Setup(x => x.GetAccountById(account.Id)).Returns(account);

            var result = _controller.GetAccount(account.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<Account>>(okResult.Value);
            Assert.Equal(account, apiResponse.Data);
        }

        [Fact]
        public void GetAccount_NonExistingId_ReturnsNotFound()
        {
            int nonExistingId = 999;

            _accountServiceMock.Setup(x => x.GetAccountById(nonExistingId)).Returns((Account)null);

            var result = _controller.GetAccount(nonExistingId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ErrorApiResponse>(notFoundResult.Value);
            Assert.Equal("Account not found", apiResponse.Message);
        }

        [Fact]
        public void Index_ReturnsOk()
        {
            var result = _controller.Index();

            var okResult = Assert.IsType<OkResult>(result);
        }
    }
}
