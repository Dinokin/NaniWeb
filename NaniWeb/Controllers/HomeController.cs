using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;
using NaniWeb.Models.Home;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly NaniWebContext _naniWebContext;
        private readonly ReCaptcha _reCaptcha;
        private readonly SettingsKeeper _settingsKeeper;

        public HomeController(IEmailSender emailSender, NaniWebContext naniWebContext, ReCaptcha reCaptcha, SettingsKeeper settingsKeeper)
        {
            _emailSender = emailSender;
            _naniWebContext = naniWebContext;
            _reCaptcha = reCaptcha;
            _settingsKeeper = settingsKeeper;
        }

        public async Task<IActionResult> Index()
        {
            var latestReleases = await _naniWebContext.Chapters.OrderByDescending(chp => chp.ReleaseDate).Take(10).Include(chp => chp.Series).ToArrayAsync();

            ViewData["LatestReleases"] = latestReleases;

            return View("Home");
        }

        public async Task<IActionResult> Announcement(string urlSlug)
        {
            var announcement = await _naniWebContext.Announcements.SingleAsync(ann => ann.UrlSlug == urlSlug);

            ViewData["Announcement"] = announcement;

            return View();
        }

        public async Task<IActionResult> Announcements()
        {
            var announcements = await _naniWebContext.Announcements.OrderByDescending(ann => ann.PostDate).ToArrayAsync();

            ViewData["Announcements"] = announcements;

            return View();
        }

        [Route("{action}/{urlSlug}")]
        public async Task<IActionResult> Project(string urlSlug)
        {
            var series = await _naniWebContext.Series.Include(srs => srs.Chapters).SingleAsync(srs => srs.UrlSlug == urlSlug);

            ViewData["Series"] = series;

            return View("Project");
        }

        [Route("{action}/{urlSlug}/{chapterNumber:decimal}")]
        public async Task<IActionResult> Project(string urlSlug, decimal chapterNumber, Series.SeriesType? mode)
        {
            var seriesList = await _naniWebContext.Series.OrderBy(srs => srs.Name).ToArrayAsync();
            var series = seriesList.Single(srs => srs.UrlSlug == urlSlug);
            var chapters = await _naniWebContext.Chapters.Where(chp => chp.Series == series).OrderByDescending(chp => chp.ChapterNumber).ToArrayAsync();
            var chapter = chapters.Single(chp => chp.ChapterNumber == chapterNumber);
            var pages = await _naniWebContext.Pages.Where(pg => pg.Chapter == chapter).OrderBy(pg => pg.PageNumber).ToArrayAsync();
            var nextChapter = chapters.LastOrDefault(chp => chp.ChapterNumber > chapter.ChapterNumber);

            ViewData["SeriesList"] = seriesList;
            ViewData["Series"] = series;
            ViewData["Chapters"] = chapters;
            ViewData["Chapter"] = chapter;
            ViewData["Pages"] = pages;
            ViewData["NextChapter"] = nextChapter;

            if (mode == null && Request.Cookies["ReaderMode"] != null)
            {
                ViewData["ReaderMode"] = Enum.Parse<Series.SeriesType>(Request.Cookies["ReaderMode"]);
            }
            else if (mode != null)
            {
                ViewData["ReaderMode"] = mode;

                Response.Cookies.Append("ReaderMode", mode.ToString());
            }
            else
            {
                ViewData["ReaderMode"] = series.Type;
            }

            return View("Read");
        }

        public async Task<IActionResult> Projects()
        {
            var series = await _naniWebContext.Series.OrderBy(srs => srs.Name).ToArrayAsync();

            ViewData["Series"] = series;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(Contact contact)
        {
            if (ModelState.IsValid && await _reCaptcha.ValidateResponse(Request.Form["g-recaptcha-response"]))
            {
                await _emailSender.SendEmailAsync($"{_settingsKeeper.GetSetting("GroupsEmailAddress").Value}", $"Message from {contact.Name}", $"{contact.Content}{Environment.NewLine}Sent by: {contact.Destination}");

                TempData["Error"] = false;
            }
            else
            {
                TempData["Error"] = true;
            }

            return RedirectToAction("Contact");
        }
    }
}