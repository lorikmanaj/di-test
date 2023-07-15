using AMS.Domain;
using AMS.Domain.Models;

namespace AMS.Application.Interfaces
{
    public interface ITransactionService : IGenericRepository<Transaction>
    {
        void CreateTransaction(int accountId, decimal amount, TransactionType transactionType);
        IEnumerable<Transaction> GetTransactionsByAccountId(int accountId);
        Transaction GetTransactionById(int transactionId);
        void AddTransaction(Transaction transaction);
        void UpdateTransaction(Transaction transaction);
    }
}
