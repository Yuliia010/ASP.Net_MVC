using ASP.Net_MVC.Core;
using ASP.Net_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace ASP.Net_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                return RedirectToAction("Login"); // login page
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


    }
}

