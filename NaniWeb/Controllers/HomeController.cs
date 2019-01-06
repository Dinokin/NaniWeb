using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly NaniWebContext _naniWebContext;

        public HomeController(NaniWebContext naniWebContext)
        {
            _naniWebContext = naniWebContext;
        }

        public IActionResult Index()
        {
            return null;
        }

        public IActionResult Announcement(string urlSlug)
        {
            return null;
        }

        public IActionResult Announcements()
        {
            return null;
        }

        [Route("{action}/{urlSlug}")]
        public IActionResult Project(string urlSlug)
        {
            return null;
        }

        [Route("{action}/{urlSlug}/{chapterNumber:decimal}")]
        public async Task<IActionResult> Project(string urlSlug, decimal chapterNumber, Series.SeriesType? mode)
        {
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.UrlSlug == urlSlug);
            var chapters = new LinkedList<Chapter>(_naniWebContext.Chapters.Where(chp => chp.Series == series).OrderByDescending(chp => chp.ChapterNumber));
            var chapter = chapters.Single(chp => chp.ChapterNumber == chapterNumber);
            var pages = await _naniWebContext.Pages.Where(pg => pg.Chapter == chapter).OrderBy(pg => pg.PageNumber).ToListAsync();
            var prevChapter = chapters.Find(chapter)?.Next?.Value;
            var nextChapter = chapters.Find(chapter)?.Previous?.Value;

            ViewData["Series"] = series;
            ViewData["Chapters"] = chapters;
            ViewData["Chapter"] = chapter;
            ViewData["Pages"] = pages;
            ViewData["prevChapter"] = prevChapter;
            ViewData["nextChapter"] = nextChapter;

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

        public IActionResult Projects()
        {
            return null;
        }

        public IActionResult About()
        {
            return null;
        }

        public IActionResult Contact()
        {
            return null;
        }
    }
}