namespace AMS.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
