using AMS.Domain.Models;

namespace AMS.Application.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccountsByUserId(int userId);
        Account GetAccountById(int accountId);
        void AddAccount(Account account);
        void CreateAccount(string name, AccountType accountType, decimal initialBalance);
        void UpdateAccount(Account account);
        void DeleteAccount(int accountId);
        IEnumerable<Account> GetAccounts();
    }
}
