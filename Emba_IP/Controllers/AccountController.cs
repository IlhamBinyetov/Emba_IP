using Emba_IP.Models;
using Emba_IP.Services;
using Emba_IP.ViewModels;
using Emba_IP.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.DirectoryServices;
using System.Reflection.PortableExecutable;

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

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "İstifadəçi adı və ya şifrə yanlışdır.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,      
                model.Password,
                false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "İstifadəçi adı və ya şifrə yanlışdır.");
            return View(model);
        }

        #region Ldap
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
        #endregion




        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(usersWithRoles);
        }

        // AccountController.cs

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                AllRoles = roles,
                SelectedRole = userRole
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> EditUser(UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Rollar silinərkən xəta baş verdi.");
                return View(model);
            }

            var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Yeni rol təyin edilərkən xəta baş verdi.");
                return View(model);
            }

            return RedirectToAction("Users");
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // Əmin olun ki, SuperAdmin özünü silə bilməsin
            if (user.UserName == "Administrator")
            {
                return BadRequest("SuperAdmin istifadəçisi silinə bilməz.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }

            // Xəta mesajlarını göstərmək üçün
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("Users", await _userManager.Users.ToListAsync());
        }

        // AccountController.cs

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult ChangeCredentials()
        {
            // View-a bir model göndərə bilərsiniz
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ChangeCredentials(ChangeCredentialsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Şifrəni dəyişmək
            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                // Şifrə dəyişikliyində xəta varsa
                return View(model);
            }

            // E-maili dəyişmək
            if (user.Email != model.NewEmail)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                var emailChangeResult = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);
                if (!emailChangeResult.Succeeded)
                {
                    // Email dəyişikliyində xəta varsa
                    return View(model);
                }
            }

            TempData["SuccessMessage"] = "Məlumatlarınız uğurla yeniləndi!";
            return RedirectToAction("Index", "Home");
        }
    }
}
