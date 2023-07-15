using AMS.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace AMS.Web.ViewModels
{
    public class TransactionRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }
    }
}
