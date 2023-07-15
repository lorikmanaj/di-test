using Microsoft.AspNetCore.Identity;

namespace AMS.Domain.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
