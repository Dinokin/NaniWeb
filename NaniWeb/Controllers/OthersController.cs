using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;
using NaniWeb.Models.SignIn;
using NaniWeb.Others;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class OthersController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly NaniWebContext _naniWebContext;
        private readonly SettingsKeeper _settingsKeeper;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;

        public OthersController(IHostingEnvironment hostingEnvironment, NaniWebContext naniWebContext, SettingsKeeper settingsKeeper, SignInManager<IdentityUser<int>> signInManager, UserManager<IdentityUser<int>> userManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _naniWebContext = naniWebContext;
            _settingsKeeper = settingsKeeper;
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
            return Utils.IsInstalled() ? (IActionResult) RedirectToAction("Home", "Home") : View();
        }

        [HttpPost]
        public async Task<IActionResult> Install(SignUpForm signUpForm)
        {
            if (Utils.IsInstalled())
                return RedirectToAction("Home", "Home");

            if (ModelState.IsValid)
            {
                var tasks = new List<Task>();

                await _naniWebContext.Database.MigrateAsync();
                _settingsKeeper.SynchronizeSettings();

                var user = new IdentityUser<int>
                {
                    UserName = signUpForm.Username,
                    Email = signUpForm.Email,
                    EmailConfirmed = true
                };

                tasks.Add(Task.Run(async () =>
                {
                    await _userManager.CreateAsync(user, signUpForm.Password);
                    await _userManager.AddToRoleAsync(user, "Administrator");
                }));
                tasks.Add(Task.Run(async () => await System.IO.File.WriteAllTextAsync($"{Utils.CurrentDirectory.FullName}{Path.DirectorySeparatorChar}INSTALLED.TXT", "DO NOT DELETE!!!!")));
                tasks.Add(Task.Run(async () =>
                {
                    var manifest = new ManifestBuilder
                    {
                        ShortName = _settingsKeeper.GetSetting("SiteName").Value,
                        Name = _settingsKeeper.GetSetting("SiteName").Value,
                        Icons = new[]
                        {
                            new ManifestBuilder.Icon
                            {
                                Src = "assets/icon.png",
                                Type = "image/png",
                                Sizes = "512x512"
                            }
                        },
                        ThemeColor = "#FFF",
                        BackgroundColor = "#FFF",
                        GcmSenderId = "103953800507"
                    };

                    await manifest.BuildManifest(_hostingEnvironment);
                }));
                await Task.WhenAll(tasks);
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