using ASP.Net_MVC.Core;
using ASP.Net_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace ASP.Net_MVC.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new AppUser { 
                 
                FullName = model.Name,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account"); // login page
            }

            foreach(var e in result.Errors)
            {
                ModelState.AddModelError("", e.Description);
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email");
                return View(model);
            }

            var signResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (signResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else if (signResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Account is blocked");
            }
            else if (signResult.RequiresTwoFactor)
            {
                ModelState.AddModelError("", "Two - factor authentication required");
            }
            else
            {
                ModelState.AddModelError("", "Invalid password");
            }
            
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> UserCabinet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            { 
                return RedirectToAction("Login"); 
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserCabinetViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? "User"
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new EditProfileViewModel { FullName = user.FullName };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.FullName = model.FullName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("UserCabinet");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("UserCabinet");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPanel()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserInfoViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);


                userRoles.Add(new UserInfoViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles.ToList()
                });
            }

            return View(userRoles);
        }
            [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction("AdminPanel");
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("AdminPanel");
            }

            return BadRequest("delete error");
        }
    }
}

