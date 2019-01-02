using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NaniWeb.Models.SignIn;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class SignInController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly SettingsKeeper _settingsKeeper;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public SignInController(IEmailSender emailSender, SettingsKeeper settingsKeeper, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager)
        {
            _emailSender = emailSender;
            _settingsKeeper = settingsKeeper;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult SignIn(string result)
        {
            ViewData["Result"] = result;

            return User.Identity.IsAuthenticated ? (IActionResult) RedirectToAction("Add", "ChapterManager") : View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInForm loginForm)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            if (!ModelState.IsValid)
                return RedirectToAction("SignIn", new {result = "Error"});

            var result = await _signInManager.PasswordSignInAsync(loginForm.Username, loginForm.Password, loginForm.Remember, true);

            return result.Succeeded ? RedirectToAction("Add", "ChapterManager") : RedirectToAction("SignIn", new {result = "Error"});
        }

        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            if (!bool.Parse(_settingsKeeper.GetSetting("EnableRegistration").Value))
                return RedirectToAction("SignIn");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpForm signUpForm)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            if (!bool.Parse(_settingsKeeper.GetSetting("EnableRegistration").Value))
                return RedirectToAction("SignIn");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = true;

                return RedirectToAction("SignUp");
            }

            var enableEmail = bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value);

            var user = new IdentityUser<int>
            {
                UserName = signUpForm.Username,
                Email = signUpForm.Email,
                EmailConfirmed = !enableEmail
            };
            var result = await _userManager.CreateAsync(user, signUpForm.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = $"https://{_settingsKeeper.GetSetting("SiteUrl")}{Url.Action("Confirm", new {userId = user.Id, code})}";
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

                return enableEmail ? RedirectToAction("SignIn", new {result = "EmailRegSent"}) : RedirectToAction("SignIn");
            }

            TempData["Error"] = true;

            return RedirectToAction("SignUp");
        }

        public async Task<IActionResult> Confirm(string userId, string code)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            if (userId == null || code == null)
                return RedirectToAction("SignIn");

            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ConfirmEmailAsync(user, code);

            return result.Succeeded ? RedirectToAction("SignIn", new {result = "EmailRegConfirmed"}) : RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            var enableEmail = bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value);

            return enableEmail ? (IActionResult) View() : RedirectToAction("SignIn");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordForm resetPassword)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            if (!bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value))
                return RedirectToAction("SignIn");

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (user == null)
                return RedirectToAction("SignIn");

            var code = _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"https://{_settingsKeeper.GetSetting("SiteUrl")}{Url.Action("NewPassword", new {userId = user.Id, code})}";
            await _emailSender.SendEmailAsync(user.Email, "Password reset requested", $"Click <a href='{callbackUrl}'>here</a> to reset your password.");

            return RedirectToAction("SignIn", new {result = "EmailResSent"});
        }

        [HttpGet]
        public async Task<IActionResult> NewPassword(string userId, string code)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            if (userId == null || code == null)
                return RedirectToAction("SignIn");

            var user = await _userManager.FindByIdAsync(userId);

            return user != null ? (IActionResult) View() : RedirectToAction("SignIn");
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(string userId, string code, NewPasswordForm newPasswordForm)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Add", "ChapterManager");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || code == null || !ModelState.IsValid)
            {
                TempData["Error"] = true;

                return RedirectToAction("NewPassword", new {userId, code});
            }

            var result = await _userManager.ResetPasswordAsync(user, code, newPasswordForm.Password);

            return result.Succeeded ? RedirectToAction("SignIn", new {result = "ResConfirm"}) : RedirectToAction("NewPassword", new {userId, code});
        }
    }
}