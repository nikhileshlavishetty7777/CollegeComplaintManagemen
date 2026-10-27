using CollegeComplaintManagement.Models;
using CollegeComplaintManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollegeComplaintManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ---------- REGISTER ----------

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Department must be one of the allowed values
            if (!RegisterViewModel.Departments.Contains(model.Department))
                ModelState.AddModelError(nameof(model.Department), "Please select a valid department.");

            if (!ModelState.IsValid)
                return View(model);

            // Duplicate email prevention
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FullName = model.FullName.Trim(),
                Department = model.Department,
                RegisteredAt = DateTime.UtcNow
            };

            // Identity hashes the password automatically
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // The role is hard-coded on the server. It is never read from the form.
                await _userManager.AddToRoleAsync(user, AppRoles.Student);

                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ---------- LOGIN ----------

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Same message as a wrong password so we don't reveal which emails exist
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Only follow local URLs (prevents open-redirect attacks)
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return await RedirectByRoleAsync(user);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Account locked after too many failed attempts. Try again in 5 minutes.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // ---------- LOGOUT (POST + anti-forgery) ----------

        [HttpPost, Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        // ---------- ACCESS DENIED ----------

        [HttpGet, AllowAnonymous]
        public IActionResult AccessDenied() => View();

        // ---------- HELPER ----------

        private async Task<IActionResult> RedirectByRoleAsync(ApplicationUser user)
        {
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Dashboard", "Complaints");
        }
    }
}
