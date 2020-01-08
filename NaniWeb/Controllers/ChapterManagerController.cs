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
using NaniWeb.Models.Chapter;
using NaniWeb.Others;
using NaniWeb.Others.Services;

namespace NaniWeb.Controllers
{
    public class ChapterManagerController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly NaniWebContext _naniWebContext;
        private readonly SettingsManager _settingsManager;

        public ChapterManagerController(IWebHostEnvironment hostingEnvironment, NaniWebContext naniWebContext, SettingsManager settingsManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _naniWebContext = naniWebContext;
            _settingsManager = settingsManager;
        }

        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        public async Task<IActionResult> List(int id)
        {
            var chapters = await _naniWebContext.Chapters.Where(chp => chp.SeriesId == id).OrderByDescending(key => key.ChapterNumber).ToArrayAsync();

            ViewData["Chapters"] = chapters;

            return View("ChapterList");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        public async Task<IActionResult> Add(int id)
        {
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == id);

            ViewData["SeriesName"] = series.Name;
            var model = new ChapterAdd
            {
                SeriesId = series.Id
            };

            return View("AddChapter", model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> Add(ChapterAdd chapterAdd)
        {
            if (ModelState.IsValid)
            {
                var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == chapterAdd.SeriesId);
                var chapter = new Chapter
                {
                    Volume = chapterAdd.Volume ?? 0,
                    ChapterNumber = chapterAdd.ChapterNumber,
                    Name = chapterAdd.Name ?? string.Empty,
                    SeriesId = series.Id,
                    Pages = new List<Page>(),
                    ReleaseDate = DateTime.UtcNow
                };

                var temp = Utils.CurrentDirectory.CreateSubdirectory($"Temp{Path.DirectorySeparatorChar}{Guid.NewGuid()}");
                var tempPages = temp.CreateSubdirectory("Pages");
                var destination = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
                var pagesZip = $"{temp.FullName}{Path.DirectorySeparatorChar}pages.zip";
                Directory.CreateDirectory(destination);

                using (var stream = System.IO.File.Create(pagesZip))
                {
                    await chapterAdd.Pages.CopyToAsync(stream);
                }

                ZipFile.ExtractToDirectory(pagesZip, tempPages.FullName);
                var pageList = tempPages.EnumerateFiles().Where(fl => fl.Extension == ".png").OrderBy(fl => fl.Name.Length).ThenBy(fl => fl.Name).ToList();

                if (pageList.Count == 0)
                {
                    temp.Delete(true);

                    TempData["Error"] = true;

                    return RedirectToAction("Add");
                }

                for (var i = 0; i < pageList.Count; i++)
                {
                    var page = new Page
                    {
                        PageNumber = i
                    };

                    do
                    {
                        page.Id = Guid.NewGuid();
                    } while (System.IO.File.Exists($"{destination}{page.Id}.png"));

                    chapter.Pages.Add(page);

                    pageList[i].CopyTo($"{destination}{page.Id}.png");
                }

                await _naniWebContext.Chapters.AddAsync(chapter);
                await _naniWebContext.SaveChangesAsync();

                temp.Delete(true);

                return RedirectToAction("List", "SeriesManager", new {id = series.Id});
            }

            TempData["Error"] = true;

            return RedirectToAction("Add");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            var chapter = await _naniWebContext.Chapters.SingleAsync(chp => chp.Id == id);
            var model = new ChapterEdit
            {
                ChapterId = chapter.Id,
                Volume = chapter.Volume,
                ChapterNumber = chapter.ChapterNumber,
                Name = chapter.Name,
            };

            return View("EditChapter", model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> Edit(ChapterEdit chapterEdit)
        {
            if (ModelState.IsValid)
            {
                var chapter = await _naniWebContext.Chapters.SingleAsync(chp => chp.Id == chapterEdit.ChapterId);
                chapter.Volume = chapterEdit.Volume ?? 0;
                chapter.ChapterNumber = chapterEdit.ChapterNumber;
                chapter.Name = chapterEdit.Name ?? string.Empty;

                if (chapterEdit.Pages != null)
                {
                    var temp = Utils.CurrentDirectory.CreateSubdirectory($"Temp{Path.DirectorySeparatorChar}{Guid.NewGuid()}");
                    var tempPages = temp.CreateSubdirectory("Pages");
                    var destination = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
                    var pagesZip = $"{temp.FullName}{Path.DirectorySeparatorChar}pages.zip";
                    var pages = _naniWebContext.Pages.Where(pg => pg.Chapter == chapter);
                    var downloadsDir = Utils.CurrentDirectory.CreateSubdirectory("Downloads");
                    var downloadFile = $"{downloadsDir.FullName}{Path.DirectorySeparatorChar}{chapter.Id}.zip";

                    foreach (var page in pages)
                        System.IO.File.Delete($"{destination}{page.Id}.png");

                    _naniWebContext.Pages.RemoveRange(pages);

                    using (var stream = System.IO.File.Create(pagesZip))
                    {
                        await chapterEdit.Pages.CopyToAsync(stream);
                    }

                    ZipFile.ExtractToDirectory(pagesZip, tempPages.FullName);
                    var pageList = tempPages.EnumerateFiles().Where(fl => fl.Extension == ".png").OrderBy(fl => fl.Name.Length).ThenBy(fl => fl.Name).ToList();
                    chapter.Pages = new List<Page>();


                    for (var i = 0; i < pageList.Count; i++)
                    {
                        var page = new Page
                        {
                            PageNumber = i
                        };

                        do
                        {
                            page.Id = Guid.NewGuid();
                        } while (System.IO.File.Exists($"{destination}{page.Id}.png"));

                        chapter.Pages.Add(page);

                        pageList[i].CopyTo($"{destination}{page.Id}.png");

                        System.IO.File.Delete(downloadFile);
                    }

                    temp.Delete(true);
                }

                _naniWebContext.Chapters.Update(chapter);
                await _naniWebContext.SaveChangesAsync();

                return RedirectToAction("List", new {id = chapter.SeriesId});
            }

            TempData["Error"] = true;

            return RedirectToAction("Add");
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int id)
        {
            var chapter = await _naniWebContext.Chapters.Include(chp => chp.Pages).SingleAsync(chp => chp.Id == id);
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == chapter.SeriesId);
            var destination = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
            var downloadsDir = Utils.CurrentDirectory.CreateSubdirectory("Downloads");
            var file = $"{downloadsDir.FullName}{Path.DirectorySeparatorChar}{chapter.Id}.zip";

            foreach (var page in chapter.Pages)
                System.IO.File.Delete($"{destination}{page.Id}.png");

            System.IO.File.Delete(file);

            _naniWebContext.Chapters.Remove(chapter);
            await _naniWebContext.SaveChangesAsync();

            return RedirectToAction("List", new {id = series.Id});
        }
    }
}