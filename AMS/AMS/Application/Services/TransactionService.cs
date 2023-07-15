using AMS.Application.Interfaces;
using AMS.Data;
using AMS.Domain;
using AMS.Domain.Models;
using AMS.Infrastructure.Repositories;

namespace AMS.Application.Services
{
    public class TransactionService : GenericRepository<Transaction>, ITransactionService
    {
        private readonly IGenericRepository<Transaction> _transactionRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly AMSDb _dbContext;
        public TransactionService(IGenericRepository<Transaction> transactionRepository, IGenericRepository<Account> accountRepository,
            AMSDb dbContext)
            : base(dbContext)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _dbContext = dbContext;
        }

        public void CreateTransaction(int accountId, decimal amount, TransactionType transactionType)
        {
            var account = _accountRepository.GetById(accountId);

            if (account == null)
                throw new ArgumentException("Account not found");

            var transaction = new Transaction
            {
                Date = DateTime.Now,
                Amount = amount,
                Type = transactionType,
                // Set props
            };

            if (transactionType == TransactionType.Out && account.Balance < amount)
                throw new InvalidOperationException("Insufficient balance");

            if (transactionType == TransactionType.Out)
                account.Balance -= amount;
            else if (transactionType == TransactionType.In)
                account.Balance += amount;

            account.Transactions.Add(transaction);

            _transactionRepository.Add(transaction);
            _transactionRepository.SaveChanges();
        }

        public IEnumerable<Transaction> GetTransactionsByAccountId(int accountId)
        {
            return _transactionRepository
                .GetAll()
                .Where(transaction => transaction.AccountId == accountId)
                .ToList();
        }

        public Transaction GetTransactionById(int transactionId)
        {
            return _transactionRepository.GetById(transactionId);
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactionRepository.Add(transaction);
            _transactionRepository.SaveChanges();
        }

        public void UpdateTransaction(Transaction transaction)
        {
            _transactionRepository.Update(transaction);
            _transactionRepository.SaveChanges();
        }
    }
}
