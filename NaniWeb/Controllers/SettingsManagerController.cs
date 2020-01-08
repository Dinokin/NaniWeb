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
        private readonly SettingsManager _settingsManager;

        public SettingsManagerController(IWebHostEnvironment hostingEnvironment, SettingsManager settingsManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _settingsManager = settingsManager;
        }

        [HttpGet]
        public IActionResult General()
        {
            var model = new GeneralForm
            {
                SiteName = _settingsManager.GetSetting("SiteName").Value,
                SiteDescription = _settingsManager.GetSetting("SiteDescription").Value,
                SiteUrl = _settingsManager.GetSetting("SiteUrl").Value,
                EnableRegistration = bool.Parse(_settingsManager.GetSetting("EnableRegistration").Value),
                NumberOfUpdatesToShow = byte.Parse(_settingsManager.GetSetting("NumberOfUpdatesToShow").Value),
                SiteFooter = _settingsManager.GetSetting("SiteFooterCode").Value,
                SiteSideBar = _settingsManager.GetSetting("SiteSideBar").Value,
                SiteAboutPage = _settingsManager.GetSetting("SiteAboutPage").Value
            };

            return View("GeneralSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> General(GeneralForm generalForm)
        {
            if (ModelState.IsValid)
            {
                var tasks = new Task[8];

                tasks[0] = Task.Run(() => _settingsManager.AddSettings("SiteName", generalForm.SiteName));
                tasks[1] = Task.Run(() => _settingsManager.AddSettings("SiteDescription", generalForm.SiteDescription));
                tasks[2] = Task.Run(() => _settingsManager.AddSettings("SiteUrl", generalForm.SiteUrl));
                tasks[3] = Task.Run(() => _settingsManager.AddSettings("EnableRegistration", generalForm.EnableRegistration.ToString()));
                tasks[4] = Task.Run(() => _settingsManager.AddSettings("NumberOfUpdatesToShow", generalForm.NumberOfUpdatesToShow.ToString()));
                tasks[5] = Task.Run(() => _settingsManager.AddSettings("SiteFooterCode", generalForm.SiteFooter ?? string.Empty));
                tasks[6] = Task.Run(() => _settingsManager.AddSettings("SiteSideBar", generalForm.SiteSideBar ?? string.Empty));
                tasks[7] = Task.Run(() => _settingsManager.AddSettings("SiteAboutPage", generalForm.SiteAboutPage ?? string.Empty));

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
                EnableEmailRecovery = bool.Parse(_settingsManager.GetSetting("EnableEmailRecovery").Value),
                SmtpServer = _settingsManager.GetSetting("SmtpServer").Value,
                SmtpUser = _settingsManager.GetSetting("SmtpUser").Value,
                SmtpPassword = _settingsManager.GetSetting("SmtpPassword").Value,
                SiteEmail = _settingsManager.GetSetting("GroupsEmailAddress").Value,
                RecaptchaSiteKey = _settingsManager.GetSetting("RecaptchaSiteKey").Value,
                RecaptchaSecretKey = _settingsManager.GetSetting("RecaptchaSecretKey").Value
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
                    tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableEmailRecovery", emailForm.EnableEmailRecovery.ToString()));
                    tasks[1] = Task.Run(() => _settingsManager.AddSettings("SmtpServer", emailForm.SmtpServer));
                    tasks[2] = Task.Run(() => _settingsManager.AddSettings("SmtpUser", emailForm.SmtpUser));
                    tasks[3] = Task.Run(() => _settingsManager.AddSettings("SmtpPassword", emailForm.SmtpPassword));
                    tasks[4] = Task.Run(() => _settingsManager.AddSettings("GroupsEmailAddress", emailForm.SiteEmail));
                    tasks[5] = Task.Run(() => _settingsManager.AddSettings("RecaptchaSiteKey", emailForm.RecaptchaSiteKey));
                    tasks[6] = Task.Run(() => _settingsManager.AddSettings("RecaptchaSecretKey", emailForm.RecaptchaSecretKey));

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
                tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableEmailRecovery", emailForm.EnableEmailRecovery.ToString()));
                tasks[1] = Task.Run(() => _settingsManager.AddSettings("SmtpServer", emailForm.SmtpServer ?? string.Empty));
                tasks[2] = Task.Run(() => _settingsManager.AddSettings("SmtpUser", emailForm.SmtpUser ?? string.Empty));
                tasks[3] = Task.Run(() => _settingsManager.AddSettings("SmtpPassword", emailForm.SmtpPassword ?? string.Empty));
                tasks[4] = Task.Run(() => _settingsManager.AddSettings("GroupsEmailAddress", emailForm.SiteEmail));
                tasks[5] = Task.Run(() => _settingsManager.AddSettings("RecaptchaSiteKey", emailForm.RecaptchaSiteKey));
                tasks[6] = Task.Run(() => _settingsManager.AddSettings("RecaptchaSecretKey", emailForm.RecaptchaSecretKey));

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
                EnableGoogleAnalytics = bool.Parse(_settingsManager.GetSetting("EnableGoogleAnalytics").Value),
                GoogleAnalyticsTrackingCode = _settingsManager.GetSetting("GoogleAnalyticsTrackingCode").Value
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
                    tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableGoogleAnalytics", googleAnalyticsForm.EnableGoogleAnalytics.ToString()));
                    tasks[1] = Task.Run(() => _settingsManager.AddSettings("GoogleAnalyticsTrackingCode", googleAnalyticsForm.GoogleAnalyticsTrackingCode));

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
                tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableGoogleAnalytics", googleAnalyticsForm.EnableGoogleAnalytics.ToString()));
                tasks[1] = Task.Run(() => _settingsManager.AddSettings("GoogleAnalyticsTrackingCode", googleAnalyticsForm.GoogleAnalyticsTrackingCode ?? string.Empty));

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
                EnableDisqus = bool.Parse(_settingsManager.GetSetting("EnableDisqus").Value),
                DisqusShortname = _settingsManager.GetSetting("DisqusShortname").Value
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
                    tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableDisqus", disqusForm.EnableDisqus.ToString()));
                    tasks[1] = Task.Run(() => _settingsManager.AddSettings("DisqusShortname", disqusForm.DisqusShortname));

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
                tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableDisqus", disqusForm.EnableDisqus.ToString()));
                tasks[1] = Task.Run(() => _settingsManager.AddSettings("DisqusShortname", disqusForm.DisqusShortname ?? string.Empty));

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
                EnableAds = bool.Parse(_settingsManager.GetSetting("EnableAds").Value),
                AdsHeaderCode = _settingsManager.GetSetting("AdsHeaderCode").Value,
                AdsLocationTop = _settingsManager.GetSetting("AdsLocationTop").Value,
                AdsLocationMiddle = _settingsManager.GetSetting("AdsLocationMiddle").Value,
                AdsLocationBottom = _settingsManager.GetSetting("AdsLocationBottom").Value
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
                    tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableAds", adsForm.EnableAds.ToString()));
                    tasks[1] = Task.Run(() => _settingsManager.AddSettings("AdsHeaderCode", adsForm.AdsHeaderCode ?? string.Empty));
                    tasks[2] = Task.Run(() => _settingsManager.AddSettings("AdsLocationTop", adsForm.AdsLocationTop ?? string.Empty));
                    tasks[3] = Task.Run(() => _settingsManager.AddSettings("AdsLocationMiddle", adsForm.AdsLocationMiddle ?? string.Empty));
                    tasks[4] = Task.Run(() => _settingsManager.AddSettings("AdsLocationBottom", adsForm.AdsLocationBottom ?? string.Empty));

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
                tasks[0] = Task.Run(() => _settingsManager.AddSettings("EnableAds", adsForm.EnableAds.ToString()));
                tasks[1] = Task.Run(() => _settingsManager.AddSettings("AdsHeaderCode", adsForm.AdsHeaderCode ?? string.Empty));
                tasks[2] = Task.Run(() => _settingsManager.AddSettings("AdsLocationTop", adsForm.AdsLocationTop ?? string.Empty));
                tasks[3] = Task.Run(() => _settingsManager.AddSettings("AdsLocationMiddle", adsForm.AdsLocationMiddle ?? string.Empty));
                tasks[4] = Task.Run(() => _settingsManager.AddSettings("AdsLocationBottom", adsForm.AdsLocationBottom ?? string.Empty));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Ads");
        }
    }
}