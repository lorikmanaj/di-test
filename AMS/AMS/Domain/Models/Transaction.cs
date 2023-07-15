namespace AMS.Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
