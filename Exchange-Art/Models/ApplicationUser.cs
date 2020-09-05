using Microsoft.AspNetCore.Identity;

namespace Exchange_Art.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Private wallet key
        public string WalletPrivateKey { get; set; }

        // Public wallet key
        public string EtheruemAddress { get; set; }
    }
}