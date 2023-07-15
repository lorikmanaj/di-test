using AMS.Application.Interfaces;
using AMS.Domain;
using AMS.Domain.Models;

namespace AMS.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<Account> _accountRepository;

        public AccountService(IGenericRepository<Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void AddAccount(Account account)
        {
            _accountRepository.Add(account);
            _accountRepository.SaveChanges();
        }

        public void CreateAccount(string name, AccountType accountType, decimal initialBalance)
        {
            var account = new Account
            {
                Name = name,
                Type = accountType,
                Balance = initialBalance,
            };

            _accountRepository.Add(account);
            _accountRepository.SaveChanges();
        }

        public void DeleteAccount(int accountId)
        {
            var account = _accountRepository.GetById(accountId);
            if (account != null)
            {
                _accountRepository.Delete(account);
                _accountRepository.SaveChanges();
            }
        }

        public Account GetAccountById(int accountId)
        {
            return _accountRepository.GetById(accountId);
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _accountRepository.GetAll();
        }

        public IEnumerable<Account> GetAccountsByUserId(int userId)
        {
            return _accountRepository
                .GetAll()
                .Where(account => account.UserId == userId)
                .ToList();
        }

        public void UpdateAccount(Account account)
        {
            _accountRepository.Update(account);
            _accountRepository.SaveChanges();
        }
    }
}
