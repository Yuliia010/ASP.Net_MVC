using ASP.Net_MVC.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASP.Net_MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDBContextIdentity>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDBContextIdentity>()
                .AddDefaultTokenProviders();


            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
                options.Lockout.MaxFailedAccessAttempts = 3; 
                options.Lockout.AllowedForNewUsers = true; 
            });


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Home/Index";
                options.AccessDeniedPath = "/Home/Index";
                options.ReturnUrlParameter = "returnUrl";
            });

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRolesAndAdminAsync(services);
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@test.com";
            string adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator"
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
