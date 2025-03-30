using Microsoft.AspNetCore.Identity;

namespace ASP.Net_MVC.Core
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
