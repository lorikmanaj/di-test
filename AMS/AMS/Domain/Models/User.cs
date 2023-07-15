using Microsoft.AspNetCore.Identity;

namespace AMS.Domain.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }
}
