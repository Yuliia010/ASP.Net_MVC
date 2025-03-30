using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_MVC.Core
{
    public class AppDBContextIdentity :IdentityDbContext<AppUser>
    {
        public AppDBContextIdentity(DbContextOptions<AppDBContextIdentity> options) : base(options)
        {

        }
    }
}
