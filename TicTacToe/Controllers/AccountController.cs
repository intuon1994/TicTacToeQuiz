using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicTacToe.Data.Identity;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager) 
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager; 
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AdminLogin() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(
                            info.LoginProvider,
                            info.ProviderKey,
                            isPersistent: false);

            if (result.Succeeded)
            {
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingUser != null)
                {
                    existingUser.LastAccess = DateTime.Now;
                    await _userManager.UpdateAsync(existingUser);
                }

                return LocalRedirect(returnUrl);
            }

            // ---------------------------
            // CreateNewUser
            // ---------------------------

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                ModelState.AddModelError("", "Email not provided by external provider.");
                return View("Login");
            }

            var user = await _userManager.FindByEmailAsync(email);
            bool isNewUser = user == null;

            if (isNewUser)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    LastAccess = DateTime.Now,
                    CreateTimestamp = DateTime.Now,
                    IsActive = true,
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View("Login");
                }
            }
            await _userManager.AddLoginAsync(user, info);
            if (isNewUser)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                await _userManager.AddToRoleAsync(user, "User");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Policy = "AdministratorOnly")]
        [HttpGet]
       
        public async Task<IActionResult> RegisterAdmin()
        {
            //สร้าง user โดยไม่ผ่านหน้า register ข้อมูลสมุมติ
            var _password = "!@admin99Tictactoe";
            var _email = "admin99Tictactoe@gmail.com";

            var user = new ApplicationUser
            {
                UserName = _email,
                Email = _email,
                PhoneNumber = "000-0000000",
                IsActive = true,
                CreateTimestamp = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, _password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Administrator"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                }

                await _userManager.AddToRoleAsync(user, "Administrator");

                ViewBag.Success = "Admin registered successfully!";
            }
            else
            {
                ViewBag.Success = string.Join(" | ", result.Errors.Select(e => e.Description));
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.UserName);

                    if (user == null)
                    {
                        ModelState.AddModelError("LoginErrorSummary", "User not found.");
                        return View(model);
                    }

                    if (!user.IsActive)
                    {
                        ModelState.AddModelError("LoginErrorSummary", "No access rights");
                        return View(model);
                    }

                    if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        ModelState.AddModelError("LoginErrorSummary", "The password is incorrect.");
                        return View(model);
                    }

                    var loginResult = await _signInManager.PasswordSignInAsync(
                        user,
                        model.Password,
                        isPersistent: false,
                        lockoutOnFailure: false
                    );

                    if (loginResult.Succeeded)
                    {
                        // Update LastAccess
                        user.LastAccess = DateTime.Now;
                        await _userManager.UpdateAsync(user);

                        if (string.IsNullOrWhiteSpace(model.ReturnUrl) || !model.ReturnUrl.StartsWith("/"))
                            model.ReturnUrl = "/";

                        return Redirect(model.ReturnUrl);
                    }

                    ModelState.AddModelError("LoginErrorSummary", "Invalid login");
                    return View(model);
                }

                return View(model);

            }
            catch (Exception ex)
            { 
                throw;
            }
        }
    }
}
