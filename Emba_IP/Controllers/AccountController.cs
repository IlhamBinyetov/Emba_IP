using Emba_IP.Models;
using Emba_IP.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using Emba_IP.Services;

namespace Emba_IP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly LdapSettings _ldapSettings;
        //private readonly LdapService _ldapService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            //_ldapSettings = ldapSettings.Value;
            //_ldapService = ldapService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Bu email ilə artıq qeydiyyat olunub.");
                    return View(model);
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name, SurName = model.Surname };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
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
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,         
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "İstifadəçi adı və ya şifrə yanlışdır.");
            return View(model);
        }



        //[HttpGet]
        //public IActionResult Login()
        //{
        //    if (User.Identity != null && User.Identity.IsAuthenticated)
        //    {

        //        return RedirectToAction("Index", "Home");
        //    }

        //    return Challenge(); 
        //}


        //Login POST
        //[HttpPost]
        // public async Task<IActionResult> Login(LoginViewModel model)
        // {
        //     if (!ModelState.IsValid)
        //         return View(model);

        //     bool ldapAuthenticated = await _ldapService.Authenticate(model.Username, model.Password);
        //     if (!ldapAuthenticated)
        //     {
        //         ModelState.AddModelError("", "LDAP ilə giriş uğursuz oldu.");
        //         return View(model);
        //     }

        //     var user = await _userManager.FindByNameAsync(model.Username);
        //     if (user == null)
        //     {
        //         user = new ApplicationUser { UserName = model.Username, Email = model.Username };
        //         var createResult = await _userManager.CreateAsync(user);
        //         if (!createResult.Succeeded)
        //         {
        //             ModelState.AddModelError("", "İstifadəçi yaradılarkən xəta baş verdi.");
        //             return View(model);
        //         }
        //     }

        //     await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
        //     return RedirectToAction("Index", "Home");

        //     //if (ModelState.IsValid)
        //     //{
        //     //    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //     //    if (result.Succeeded)
        //     //    {
        //     //        return RedirectToAction("Index", "Home");
        //     //    }
        //     //    ModelState.AddModelError("", "Yanlış Email və ya Şifrə");
        //     //}

        // }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
