using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NaniWeb.Models.Profile;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly SettingsKeeper _settingsKeeper;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public ProfileController(IEmailSender emailSender, SettingsKeeper settingsKeeper, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager)
        {
            _emailSender = emailSender;
            _settingsKeeper = settingsKeeper;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var role = (await _userManager.GetRolesAsync(user))[0];

            var model = new Profile
            {
                Username = user.UserName,
                Email = user.Email,
                Role = role ?? "None"
            };

            return View("Profile", model);
        }

        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value) ? (IActionResult) View("NewEmail") : RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(NewEmailForm newEmailForm)
        {
            if (!bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value))
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmailForm.NewEmail);

                var callbackUrl = $"{_settingsKeeper.GetSetting("SiteUrl")}{Url.Action("Confirm", "SignIn", new {userId = user.Id, code})}";
                await _emailSender.SendEmailAsync(user.Email, "Email change requested", $"Click <a href='{callbackUrl}'>here</a> to confirm your new email.");
            }
            else
                TempData["Error"] = true;

            return View("NewEmail");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("NewPassword");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(NewPasswordForm newPasswordForm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = true;

                return View("NewPassword");
            }

            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, newPasswordForm.OldPassword, newPasswordForm.NewPassword);

            TempData["Error"] = result.Succeeded;

            return View("NewPassword");
        }

        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index", "Home");
        }
    }
}