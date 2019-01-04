﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NaniWeb.Models.Settings;
using NaniWeb.Others;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SettingsManagerController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly SettingsKeeper _settingsKeeper;

        public SettingsManagerController(IHostingEnvironment hostingEnvironment, SettingsKeeper settingsKeeper)
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
                EnableRegistration = bool.Parse(_settingsKeeper.GetSetting("EnableRegistration").Value)
            };

            return View("GeneralSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> General(GeneralForm generalForm)
        {
            if (ModelState.IsValid)
            {
                var tasks = new Task[5];
                var manifest = new ManifestBuilder
                {
                    ShortName = generalForm.SiteName,
                    Name = generalForm.SiteName,
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

                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteName", generalForm.SiteName));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteDescription", generalForm.SiteDescription));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("SiteUrl", generalForm.SiteUrl));
                tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableRegistration", generalForm.EnableRegistration.ToString()));
                tasks[4] = Task.Run(async () => await manifest.BuildManifest(_hostingEnvironment));

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
                SmtpPassword = _settingsKeeper.GetSetting("SmtpPassword").Value
            };

            return View("EmailSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Email(EmailForm emailForm)
        {
            var tasks = new Task[4];

            if (emailForm.EnableEmailRecovery)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableEmailRecovery", emailForm.EnableEmailRecovery.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpServer", emailForm.SmtpServer));
                    tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpUser", emailForm.SmtpUser));
                    tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("SmtpPassword", emailForm.SmtpPassword));

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

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Email");
        }

        [HttpGet]
        public IActionResult Discord()
        {
            var model = new DiscordForm
            {
                EnableDiscordBot = bool.Parse(_settingsKeeper.GetSetting("EnableDiscordBot").Value),
                DiscordToken = _settingsKeeper.GetSetting("DiscordToken").Value,
                DiscordChannelId = ulong.Parse(_settingsKeeper.GetSetting("DiscordChannelId").Value)
            };

            return View("DiscordSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Discord(DiscordForm discordForm)
        {
            var tasks = new Task[3];

            if (discordForm.EnableDiscordBot)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableDiscordBot", discordForm.EnableDiscordBot.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("DiscordToken", discordForm.DiscordToken));
                    tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("DiscordChannelId", discordForm.DiscordChannelId.ToString()));

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
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableDiscordBot", discordForm.EnableDiscordBot.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("DiscordToken", discordForm.DiscordToken ?? string.Empty));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("DiscordChannelId", discordForm.DiscordChannelId.ToString()));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Discord");
        }

        [HttpGet]
        public IActionResult Mangadex()
        {
            var model = new MangadexForm
            {
                EnableMangadexAutoUpload = bool.Parse(_settingsKeeper.GetSetting("EnableMangadexAutoUpload").Value),
                MangadexUser = _settingsKeeper.GetSetting("MangadexUser").Value,
                MangadexPassword = _settingsKeeper.GetSetting("MangadexPassword").Value,
                MangadexGroupId = int.Parse(_settingsKeeper.GetSetting("MangadexGroupId").Value)
            };

            return View("MangadexSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Mangadex(MangadexForm mangadexForm)
        {
            var tasks = new Task[4];

            if (mangadexForm.EnableMangadexAutoUpload)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableMangadexAutoUpload", mangadexForm.EnableMangadexAutoUpload.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("MangadexUser", mangadexForm.MangadexUser));
                    tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("MangadexPassword", mangadexForm.MangadexPassword));
                    tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("MangadexGroupId", mangadexForm.MangadexGroupId.ToString()));

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
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableMangadexAutoUpload", mangadexForm.EnableMangadexAutoUpload.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("MangadexUser", mangadexForm.MangadexUser ?? string.Empty));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("MangadexPassword", mangadexForm.MangadexPassword ?? string.Empty));
                tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("MangadexGroupId", mangadexForm.MangadexGroupId.ToString()));

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Mangadex");
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
        public IActionResult Fcm()
        {
            var model = new FcmForm
            {
                EnableFcm = bool.Parse(_settingsKeeper.GetSetting("EnableFcm").Value),
                FcmApiKey = _settingsKeeper.GetSetting("FcmApiKey").Value,
                FcmProjectId = _settingsKeeper.GetSetting("FcmProjectId").Value,
                FcmSenderId = ulong.Parse(_settingsKeeper.GetSetting("FcmSenderId").Value)
            };

            return View("FcmSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Fcm(FcmForm fcmForm)
        {
            if (fcmForm.EnableFcm)
            {
                if (ModelState.IsValid)
                {
                    var tasks = new Task[6];

                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableFcm", fcmForm.EnableFcm.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("FcmApiKey", fcmForm.FcmApiKey));
                    tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("FcmProjectId", fcmForm.FcmProjectId));
                    tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("FcmSenderId", fcmForm.FcmSenderId.ToString()));
                    tasks[4] = Task.Run(async () =>
                    {
                        using (var stream = System.IO.File.Create($"{Utils.CurrentDirectory.FullName}{Path.DirectorySeparatorChar}fcmkey.json"))
                        {
                            await fcmForm.FcmKeyFile.CopyToAsync(stream);
                        }
                    });
                    tasks[5] = Task.Run(async () => await Utils.BuildServiceWorker(fcmForm.FcmApiKey, fcmForm.FcmProjectId, fcmForm.FcmSenderId.ToString(), _hostingEnvironment));

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
                var tasks = new Task[4];

                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableFcm", fcmForm.EnableFcm.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("FcmApiKey", fcmForm.FcmApiKey ?? string.Empty));
                tasks[2] = Task.Run(async () => await _settingsKeeper.AddSettings("FcmProjectId", fcmForm.FcmProjectId ?? string.Empty));
                tasks[3] = Task.Run(async () => await _settingsKeeper.AddSettings("FcmSenderId", fcmForm.FcmSenderId.ToString()));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Fcm");
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
        public IActionResult Facebook()
        {
            var model = new FacebookForm
            {
                EnableFacebookPosting = bool.Parse(_settingsKeeper.GetSetting("EnableFacebookPosting").Value),
                FacebookApiKey = _settingsKeeper.GetSetting("FacebookApiKey").Value
            };

            return View("FacebookSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> Facebook(FacebookForm facebookForm)
        {
            var tasks = new Task[2];

            if (facebookForm.EnableFacebookPosting)
            {
                if (ModelState.IsValid)
                {
                    tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableFacebookPosting", facebookForm.EnableFacebookPosting.ToString()));
                    tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("FacebookApiKey", facebookForm.FacebookApiKey));

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
                tasks[0] = Task.Run(async () => await _settingsKeeper.AddSettings("EnableFacebookPosting", facebookForm.EnableFacebookPosting.ToString()));
                tasks[1] = Task.Run(async () => await _settingsKeeper.AddSettings("FacebookApiKey", facebookForm.FacebookApiKey ?? string.Empty));

                TempData["Error"] = false;

                await Task.WhenAll(tasks);
            }

            return RedirectToAction("Facebook");
        }
    }
}