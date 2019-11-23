using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;
using NaniWeb.Models.Home;
using NaniWeb.Others;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly EmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly NaniWebContext _naniWebContext;
        private readonly ReCaptcha _reCaptcha;
        private readonly SettingsKeeper _settingsKeeper;

        public HomeController(IWebHostEnvironment webHostEnvironment, EmailSender emailSender, NaniWebContext naniWebContext, ReCaptcha reCaptcha, SettingsKeeper settingsKeeper)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _naniWebContext = naniWebContext;
            _reCaptcha = reCaptcha;
            _settingsKeeper = settingsKeeper;
        }

        public async Task<IActionResult> Index()
        {
            var updateCount = int.Parse(_settingsKeeper.GetSetting("NumberOfUpdatesToShow").Value);
            ViewData["LatestReleases"] = await _naniWebContext.Chapters.OrderByDescending(chp => chp.ReleaseDate).Take(updateCount).Include(chp => chp.Series).ToArrayAsync();

            return View("Home");
        }

        public async Task<IActionResult> Announcement(string urlSlug)
        {
            ViewData["Announcement"] = await _naniWebContext.Announcements.SingleOrDefaultAsync(ann => ann.UrlSlug == urlSlug);

            if (ViewData["Announcement"] == null)
                return RedirectToAction("Announcements");

            return View();
        }

        public async Task<IActionResult> Announcements()
        {
            ViewData["Announcements"] = await _naniWebContext.Announcements.OrderByDescending(ann => ann.PostDate).ToArrayAsync();

            return View();
        }

        [Route("{action}/{urlSlug}")]
        public async Task<IActionResult> Project(string urlSlug)
        {
            var series = await _naniWebContext.Series.Include(srs => srs.Chapters).Include(srs => srs.MangadexInfo).SingleOrDefaultAsync(srs => srs.UrlSlug == urlSlug);

            if (series == null)
                return RedirectToAction("Projects");

            series.Chapters = series.Chapters.OrderByDescending(chp => chp.ChapterNumber).ToList();
            ViewData["Series"] = series;

            return View("Project");
        }

        [Route("{action}/{urlSlug}/{chapterNumber:decimal}")]
        public async Task<IActionResult> Project(string urlSlug, decimal chapterNumber, Series.SeriesType? mode)
        {
            var seriesList = await _naniWebContext.Series.OrderBy(srs => srs.Name).ToArrayAsync();
            var series = seriesList.SingleOrDefault(srs => srs.UrlSlug == urlSlug);

            if (series == null)
                return RedirectToAction("Projects");

            var chapters = new LinkedList<Chapter>(_naniWebContext.Chapters.Where(chp => chp.Series == series).OrderByDescending(chp => chp.ChapterNumber));
            var chapter = chapters.SingleOrDefault(chp => chp.ChapterNumber == chapterNumber);

            if (chapter == null)
                return RedirectToAction("Project", new {urlSlug, chapterNumber = string.Empty});

            ViewData["SeriesList"] = seriesList;
            ViewData["Series"] = series;
            ViewData["Chapters"] = chapters;
            ViewData["Chapter"] = chapter;
            ViewData["Pages"] = await _naniWebContext.Pages.Where(pg => pg.Chapter == chapter).OrderBy(pg => pg.PageNumber).ToArrayAsync();
            ViewData["PrevChapter"] = chapters.Find(chapter)?.Next?.Value;
            ViewData["NextChapter"] = chapters.Find(chapter)?.Previous?.Value;

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

        public async Task<FileStreamResult> Download(string urlSlug, decimal chapterNumber)
        {
            var chapter = await _naniWebContext.Chapters.Include(chp => chp.Series).Include(chp => chp.Pages).SingleOrDefaultAsync(chp => chp.Series.UrlSlug == urlSlug && chp.ChapterNumber == chapterNumber);

            if (chapter == null)
            {
                var bytes = System.IO.File.OpenRead($"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}assets{Path.DirectorySeparatorChar}facepalm.png");
                return File(bytes, "image/png", "error.png");
            }

            chapter.Pages = chapter.Pages.OrderBy(pg => pg.PageNumber).ToList();
            var downloadsDir = Utils.CurrentDirectory.CreateSubdirectory("Downloads");
            var file = $"{downloadsDir.FullName}{Path.DirectorySeparatorChar}{chapter.Id}.zip";
            var prepSiteName = $"[{_settingsKeeper.GetSetting("SiteName").Value.Replace(" ", "_")}]";
            var prepSeriesName = chapter.Series.Name.Replace(" ", "_");

            if (System.IO.File.Exists(file))
            {
                var bytes = System.IO.File.OpenRead(file);
                return File(bytes, "application/zip", $"{prepSiteName}{prepSeriesName}_{chapterNumber}.zip");
            }
            else
            {
                var temp = Utils.CurrentDirectory.CreateSubdirectory($"Temp{Path.DirectorySeparatorChar}{Guid.NewGuid()}");
                var pagesOrigin = $"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";

                for (var i = 0; i < chapter.Pages.Count; i++) System.IO.File.Copy($"{pagesOrigin}{chapter.Pages[i].Id}.png", $"{temp.FullName}{Path.DirectorySeparatorChar}{i}.png");

                ZipFile.CreateFromDirectory(temp.FullName, file, CompressionLevel.Optimal, false);

                temp.Delete(true);

                var bytes = System.IO.File.OpenRead(file);
                return File(bytes, "application/zip", $"{prepSiteName}{prepSeriesName}_{chapterNumber}.zip");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Projects()
        {
            var model = new ProjectSearch
            {
                Name = string.Empty,
                Status = Series.SeriesStatus.Ongoing
            };

            ViewData["Series"] = await _naniWebContext.Series.OrderBy(srs => srs.Status).ThenBy(srs => srs.Name).ToArrayAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Projects(ProjectSearch projectSearch)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(projectSearch.Name))
                    ViewData["Series"] = await _naniWebContext.Series.Where(srs => srs.Status == projectSearch.Status).OrderBy(srs => srs.Name).ToArrayAsync();
                else
                    ViewData["Series"] = await _naniWebContext.Series.Where(srs => srs.Name.ToLowerInvariant().Contains(projectSearch.Name.ToLowerInvariant()) && srs.Status == projectSearch.Status)
                        .OrderBy(srs => srs.Name).ToArrayAsync();
            }
            else
            {
                ViewData["Series"] = await _naniWebContext.Series.OrderBy(srs => srs.Status).ThenBy(srs => srs.Name).ToArrayAsync();
            }


            return View(projectSearch);
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