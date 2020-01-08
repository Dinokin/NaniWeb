using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NaniWeb.Models.Settings;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SettingsManagerController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SettingsKeeper _settingsKeeper;

        public SettingsManagerController(IWebHostEnvironment hostingEnvironment, SettingsKeeper settingsKeeper)
        {
            _hostingEnvironment = hostingEnvironment;
            _settingsKeeper = settingsKeeper;
        }

        [HttpGet]
        public IActionResult General()
        {
            var model = new GeneralForm
            {
                SiteName = _settingsKeeper.GetSetting("SiteName").Value,
                SiteDescription = _settingsKeeper.GetSetting("SiteDescription").Value,
                SiteUrl = _settingsKeeper.GetSetting("SiteUrl").Value,
                EnableRegistration = bool.Parse(_settingsKeeper.GetSetting("EnableRegistration").Value),
                NumberOfUpdatesToShow = byte.Parse(_settingsKeeper.GetSetting("NumberOfUpdatesToShow").Value),
                SiteFooter = _settingsKeeper.GetSetting("SiteFooterCode").Value,
                SiteSideBar = _settingsKeeper.GetSetting("SiteSideBar").Value,
                SiteAboutPage = _settingsKeeper.GetSetting("SiteAboutPage").Value
            };

            return View("GeneralSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> General(GeneralForm generalForm)
        {
            if (ModelState.IsValid)
            {
                var tasks = new Task[8];

                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteName", generalForm.SiteName));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteDescription", generalForm.SiteDescription));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteUrl", generalForm.SiteUrl));
                tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableRegistration", generalForm.EnableRegistration.ToString()));
                tasks[4] = Task.Run(async () => await _settingsKeeper.AddSettings("NumberOfUpdatesToShow", generalForm.NumberOfUpdatesToShow.ToString()));
                tasks[5] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteFooterCode", generalForm.SiteFooter ?? string.Empty));
                tasks[6] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteSideBar", generalForm.SiteSideBar ?? string.Empty));
                tasks[7] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteAboutPage", generalForm.SiteAboutPage ?? string.Empty));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }
            else
            {
                TempData["Error"] = true;
            }

            return RedirectToAction("General");
        }

        [HttpGet]
        public IActionResult Email()
        {
            var model = new EmailForm
            {
                EnableEmailRecovery = bool.Parse(_settingsKeeper.GetSetting("EnableEmailRecovery").Value),
                SmtpServer = _settingsKeeper.GetSetting("SmtpServer").Value,
                SmtpUser = _settingsKeeper.GetSetting("SmtpUser").Value,
                SmtpPassword = _settingsKeeper.GetSetting("SmtpPassword").Value,
                SiteEmail = _settingsKeeper.GetSetting("GroupsEmailAddress").Value,
                RecaptchaSiteKey = _settingsKeeper.GetSetting("RecaptchaSiteKey").Value,
                RecaptchaSecretKey = _settingsKeeper.GetSetting("RecaptchaSecretKey").Value
            };

            return View("EmailSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Email(EmailForm emailForm)
        {
            var tasks = new Task[7];

            if (emailForm.EnableEmailRecovery)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableEmailRecovery", emailForm.EnableEmailRecovery.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpServer", emailForm.SmtpServer));
                    tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpUser", emailForm.SmtpUser));
                    tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpPassword", emailForm.SmtpPassword));
                    tasks[4] = Task.Run(async () => await _settingsKeeper.AddSettings("GroupsEmailAddress", emailForm.SiteEmail));
                    tasks[5] = Task.Run(async () => await _settingsKeeper.AddSettings("RecaptchaSiteKey", emailForm.RecaptchaSiteKey));
                    tasks[6] = Task.Run(async () => await _settingsKeeper.AddSettings("RecaptchaSecretKey", emailForm.RecaptchaSecretKey));

                    TempData["Error"] = false;

                    await Task.WhenAll(tasks);
                }
                else
                {
                    TempData["Error"] = true;
                }
            }
            else
            {
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableEmailRecovery", emailForm.EnableEmailRecovery.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpServer", emailForm.SmtpServer ?? string.Empty));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpUser", emailForm.SmtpUser ?? string.Empty));
                tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpPassword", emailForm.SmtpPassword ?? string.Empty));
                tasks[4] = Task.Run(async () => await _settingsKeeper.AddSettings("GroupsEmailAddress", emailForm.SiteEmail));
                tasks[5] = Task.Run(async () => await _settingsKeeper.AddSettings("RecaptchaSiteKey", emailForm.RecaptchaSiteKey));
                tasks[6] = Task.Run(async () => await _settingsKeeper.AddSettings("RecaptchaSecretKey", emailForm.RecaptchaSecretKey));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Email");
        }

        [HttpGet]
        public IActionResult GoogleAnalytics()
        {
            var model = new GoogleAnalyticsForm
            {
                EnableGoogleAnalytics = bool.Parse(_settingsKeeper.GetSetting("EnableGoogleAnalytics").Value),
                GoogleAnalyticsTrackingCode = _settingsKeeper.GetSetting("GoogleAnalyticsTrackingCode").Value
            };

            return View("GoogleAnalyticsSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> GoogleAnalytics(GoogleAnalyticsForm googleAnalyticsForm)
        {
            var tasks = new Task[2];

            if (googleAnalyticsForm.EnableGoogleAnalytics)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableGoogleAnalytics", googleAnalyticsForm.EnableGoogleAnalytics.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("GoogleAnalyticsTrackingCode", googleAnalyticsForm.GoogleAnalyticsTrackingCode));

                    TempData["Error"] = false;

                    await Task.WhenAll(tasks);
                }
                else
                {
                    TempData["Error"] = true;
                }
            }
            else
            {
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableGoogleAnalytics", googleAnalyticsForm.EnableGoogleAnalytics.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("GoogleAnalyticsTrackingCode", googleAnalyticsForm.GoogleAnalyticsTrackingCode ?? string.Empty));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("GoogleAnalytics");
        }

        [HttpGet]
        public IActionResult Disqus()
        {
            var model = new DisqusForm
            {
                EnableDisqus = bool.Parse(_settingsKeeper.GetSetting("EnableDisqus").Value),
                DisqusShortname = _settingsKeeper.GetSetting("DisqusShortname").Value
            };

            return View("DisqusSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Disqus(DisqusForm disqusForm)
        {
            var tasks = new Task[2];

            if (disqusForm.EnableDisqus)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableDisqus", disqusForm.EnableDisqus.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("DisqusShortname", disqusForm.DisqusShortname));

                    TempData["Error"] = false;

                    await Task.WhenAll(tasks);
                }
                else
                {
                    TempData["Error"] = true;
                }
            }
            else
            {
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableDisqus", disqusForm.EnableDisqus.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("DisqusShortname", disqusForm.DisqusShortname ?? string.Empty));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Disqus");
        }

        [HttpGet]
        public IActionResult Ads()
        {
            var model = new AdsForm
            {
                EnableAds = bool.Parse(_settingsKeeper.GetSetting("EnableAds").Value),
                AdsHeaderCode = _settingsKeeper.GetSetting("AdsHeaderCode").Value,
                AdsLocationTop = _settingsKeeper.GetSetting("AdsLocationTop").Value,
                AdsLocationMiddle = _settingsKeeper.GetSetting("AdsLocationMiddle").Value,
                AdsLocationBottom = _settingsKeeper.GetSetting("AdsLocationBottom").Value
            };

            return View("AdsSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Ads(AdsForm adsForm)
        {
            var tasks = new Task[5];

            if (adsForm.EnableAds)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableAds", adsForm.EnableAds.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsHeaderCode", adsForm.AdsHeaderCode ?? string.Empty));
                    tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsLocationTop", adsForm.AdsLocationTop ?? string.Empty));
                    tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsLocationMiddle", adsForm.AdsLocationMiddle ?? string.Empty));
                    tasks[4] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsLocationBottom", adsForm.AdsLocationBottom ?? string.Empty));

                    TempData["Error"] = false;

                    await Task.WhenAll(tasks);
                }
                else
                {
                    TempData["Error"] = true;
                }
            }
            else
            {
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableAds", adsForm.EnableAds.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsHeaderCode", adsForm.AdsHeaderCode ?? string.Empty));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsLocationTop", adsForm.AdsLocationTop ?? string.Empty));
                tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsLocationMiddle", adsForm.AdsLocationMiddle ?? string.Empty));
                tasks[4] = Task.Run(async () => await _settingsKeeper.AddSettings("AdsLocationBottom", adsForm.AdsLocationBottom ?? string.Empty));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Ads");
        }
    }
}