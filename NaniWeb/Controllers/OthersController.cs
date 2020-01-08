using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;
using NaniWeb.Models.SignIn;
using Npgsql;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class OthersController : Controller
    {
        private readonly NaniWebContext _naniWebContext;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public OthersController(NaniWebContext naniWebContext, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager)
        {
            _naniWebContext = naniWebContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Install()
        {
            return _naniWebContext.Database.GetAppliedMigrations().Any() ? (IActionResult) RedirectToAction("Index", "Home") : View();
        }

        [HttpPost]
        public async Task<IActionResult> Install(SignUpForm signUpForm)
        {
            if (_naniWebContext.Database.GetAppliedMigrations().Any())
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                await _naniWebContext.Database.MigrateAsync();
                var npgsqlConnection = (NpgsqlConnection) _naniWebContext.Database.GetDbConnection();
                npgsqlConnection.Open();
                npgsqlConnection.ReloadTypes();

                var user = new IdentityUser<int>
                {
                    UserName = signUpForm.Username,
                    Email = signUpForm.Email,
                    EmailConfirmed = true
                };
                
                await _userManager.CreateAsync(user, signUpForm.Password);
                await _userManager.AddToRoleAsync(user, "Administrator");

                await _signInManager.PasswordSignInAsync(user, signUpForm.Password, false, false);

                return RedirectToAction("General", "SettingsManager");
            }

            TempData["Error"] = true;

            return View();
        }

        public IActionResult Denied()
        {
            return View();
        }
    }
}