﻿using AMS.Application.Interfaces;
using AMS.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Application.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class UserAccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public UserAccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAccounts()
        {
            IEnumerable<Account> accounts = _accountService.GetAccounts();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public IActionResult GetAccount(int id)
        {
            Account account = _accountService.GetAccountById(id);
            if (account == null)
                return NotFound();

            return Ok(account);
        }

        [HttpPost]
        public IActionResult CreateAccount(Account account)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _accountService.CreateAccount(account.Name, account.Type, account.Balance);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, Account account)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != account.Id)
                return BadRequest("Account ID mismatch");

            try
            {
                _accountService.UpdateAccount(account);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            Account account = _accountService.GetAccountById(id);
            if (account == null)
                return NotFound();

            _accountService.DeleteAccount(id);
            return Ok();
        }
    }
}
