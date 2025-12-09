using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicTacToe.Data.Identity;

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Login", "Account");
        //}

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}
