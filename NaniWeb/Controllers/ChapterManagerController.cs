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
        private readonly FacebookPosting _facebookPosting;
        private readonly FirebaseCloudMessaging _firebaseCloudMessaging;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly MangadexUploader _mangadexUploader;
        private readonly NaniWebContext _naniWebContext;
        private readonly SettingsKeeper _settingsKeeper;

        public ChapterManagerController(FacebookPosting facebookPosting, FirebaseCloudMessaging firebaseCloudMessaging, IHostingEnvironment hostingEnvironment, MangadexUploader mangadexUploader, NaniWebContext naniWebContext, SettingsKeeper settingsKeeper)
        {
            _facebookPosting = facebookPosting;
            _firebaseCloudMessaging = firebaseCloudMessaging;
            _hostingEnvironment = hostingEnvironment;
            _mangadexUploader = mangadexUploader;
            _naniWebContext = naniWebContext;
            _settingsKeeper = settingsKeeper;
        }

        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        public async Task<IActionResult> List(int id)
        {
            ViewData["Chapters"] = await _naniWebContext.Chapters.Where(chp => chp.SeriesId == id).OrderByDescending(key => key.ChapterNumber).ToListAsync();
            
            return View("ChapterList");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        public async Task<IActionResult> Add(int id)
        {
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == id);
            var model = new ChapterAdd
            {
                SeriesId = series.Id
            };
            
            return View("AddChapter", model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator, Uploader")]
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
                    ReleaseDate = DateTime.UtcNow,
                };

                var temp = Utils.CurrentDirectory.CreateSubdirectory($"Temp{Path.DirectorySeparatorChar}{Guid.NewGuid()}");
                var tempPages = temp.CreateSubdirectory("Pages");
                var destination = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
                var pagesZip = $"{temp.FullName}{Path.DirectorySeparatorChar}pages.zip";

                using (var stream = System.IO.File.Create(pagesZip))
                {
                    await chapterAdd.Pages.CopyToAsync(stream);
                }
                
                ZipFile.ExtractToDirectory(pagesZip, tempPages.FullName);
                var pageList = tempPages.EnumerateFiles().Where(fl => fl.Extension == ".png").OrderBy(fl => fl.Name).ToList();

                for (var i = 0; i < pageList.Count; i++)
                {
                    var page = new Page
                    {
                        Id = Guid.NewGuid(),
                        PageNumber = i
                    };

                    chapter.Pages.Add(page);

                    pageList[i].CopyTo($"{destination}{page.Id}.png");
                }

                await _naniWebContext.Chapters.AddAsync(chapter);
                await _naniWebContext.SaveChangesAsync();
                
                var siteName = _settingsKeeper.GetSetting("SiteName");
                var chapterUrl = $"{_settingsKeeper.GetSetting("SiteUrl")}{Url.Action("Project", "Home", new {urlSlug = series.UrlSlug, chapterNumber = chapter.ChapterNumber})}";
                var iconUrl = $"{_settingsKeeper.GetSetting("SiteUrl")}/assets/icon.png";
                var tasks = new Task[3];
                
                tasks[0] = Task.Run(async () =>
                {
                    var mangadexSeries = await _naniWebContext.MangadexSeries.SingleOrDefaultAsync(srs => srs.SeriesId == series.Id);

                    MangadexChapter mangadexChapter = null;
                    
                    if (chapterAdd.UploadToMangadex && mangadexSeries.MangadexId > 0)
                    {
                        using (var stream = System.IO.File.OpenRead(pagesZip))
                        {
                            mangadexChapter = await _mangadexUploader.UploadChapter(series, chapter, mangadexSeries, stream);
                        }
                    }

                    if (mangadexChapter == null)
                    {
                        mangadexChapter = new MangadexChapter
                        {
                            Chapter = chapter,
                            ChapterId = chapter.Id,
                            MangadexId = 0
                        };
                    }

                    await _naniWebContext.MangadexChapters.AddAsync(mangadexChapter);
                    await _naniWebContext.SaveChangesAsync();
                });
                tasks[1] = Task.Run(async () =>
                {
                    await _facebookPosting.Post($"New chapter available for {series.Name} at {siteName}! Read it here: {chapterUrl}");
                });
                tasks[2] = Task.Run(async () =>
                {
                    await _firebaseCloudMessaging.SendNotification($"New chapter available at {siteName}!", $"New chapter available for {series.Name} at {siteName}!", chapterUrl, iconUrl, series.Id.ToString());
                });

                await Task.WhenAll(tasks);
                temp.Delete(true);
                
                return RedirectToAction("List", new {id = series.Id});
            }
            
            TempData["Error"] = true;
            
            return RedirectToAction("Add");
        }
        
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            var chapter = await _naniWebContext.Chapters.SingleAsync(chp => chp.Id == id);
            var mangadex = await _naniWebContext.MangadexChapters.SingleOrDefaultAsync(chp => chp.Chapter == chapter);
            var model = new ChapterEdit
            {
                ChapterId = chapter.Id,
                Volume = chapter.Volume,
                ChapterNumber = chapter.ChapterNumber,
                Name = chapter.Name,
                MangadexId = mangadex.MangadexId
            };
            
            return View("EditChapter", model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(ChapterEdit chapterEdit)
        {
            if (ModelState.IsValid)
            {
                var chapter = await _naniWebContext.Chapters.SingleAsync(chp => chp.Id == chapterEdit.ChapterId);
                var mangadexChapter = await _naniWebContext.MangadexChapters.SingleAsync(chp => chp.Chapter == chapter);
                chapter.Volume = chapterEdit.Volume ?? 0;
                chapter.ChapterNumber = chapterEdit.ChapterNumber;
                chapter.Name = chapterEdit.Name ?? string.Empty;
                mangadexChapter.MangadexId = chapterEdit.MangadexId ?? 0;
                chapter.MangadexInfo = mangadexChapter;

                if (chapterEdit.Pages != null)
                {
                    var temp = Utils.CurrentDirectory.CreateSubdirectory($"Temp{Path.DirectorySeparatorChar}{Guid.NewGuid()}");
                    var tempPages = temp.CreateSubdirectory("Pages");
                    var destination = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
                    var pagesZip = $"{temp.FullName}{Path.DirectorySeparatorChar}pages.zip";
                    
                    var pages = _naniWebContext.Pages.Where(pg => pg.Chapter == chapter);

                    foreach (var page in pages)
                        System.IO.File.Delete($"{destination}{page.Id}.png");
                    
                    _naniWebContext.Pages.RemoveRange(pages);
                                        
                    using (var stream = System.IO.File.Create(pagesZip))
                        await chapterEdit.Pages.CopyToAsync(stream);
                    
                    ZipFile.ExtractToDirectory(pagesZip, tempPages.FullName);
                    var pageList = tempPages.EnumerateFiles().Where(fl => fl.Extension == ".png").OrderBy(fl => fl.Name).ToList();
                    chapter.Pages = new List<Page>();
                    
                    
                    for (var i = 0; i < pageList.Count; i++)
                    {
                        var page = new Page
                        {
                            Id = Guid.NewGuid(),
                            PageNumber = i
                        };

                        chapter.Pages.Add(page);

                        pageList[i].CopyTo($"{destination}{page.Id}.png");
                    }

                    if (chapterEdit.UploadToMangadex && mangadexChapter.MangadexId > 0)
                    {
                        var mangadexSeries = await _naniWebContext.MangadexSeries.SingleAsync(mgdx => mgdx.SeriesId == mangadexChapter.Chapter.SeriesId);

                        using (var stream = System.IO.File.OpenRead(pagesZip))
                            await _mangadexUploader.UpdateChapter(chapter, mangadexSeries, mangadexChapter, stream);
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
            var chapter = await _naniWebContext.Chapters.SingleAsync(chp => chp.Id == id);
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == chapter.SeriesId);
            var pages = _naniWebContext.Pages.Where(pg => pg.Chapter == chapter);
            var destination = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
            
            foreach (var page in pages)
                System.IO.File.Delete($"{destination}{page.Id}.png");

            _naniWebContext.Chapters.Remove(chapter);
            await _naniWebContext.SaveChangesAsync();

            return RedirectToAction("List", new {id = series.Id});
        }
    }
}