using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NaniWeb.Models.Profile;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SettingsKeeper _settingsKeeper;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public ProfileController(SettingsKeeper settingsKeeper, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager)
        {
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
            if (ModelState.IsValid)
            {
            }

            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return null;
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