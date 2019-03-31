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
        private readonly DiscordBot _discordBot;
        private readonly FirebaseCloudMessaging _firebaseCloudMessaging;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly MangadexUploader _mangadexUploader;
        private readonly NaniWebContext _naniWebContext;
        private readonly RedditPoster _redditPoster;
        private readonly SettingsKeeper _settingsKeeper;

        public ChapterManagerController(DiscordBot discordBot, FirebaseCloudMessaging firebaseCloudMessaging, IHostingEnvironment hostingEnvironment, MangadexUploader mangadexUploader, NaniWebContext naniWebContext,
            RedditPoster redditPoster, SettingsKeeper settingsKeeper)
        {
            _discordBot = discordBot;
            _firebaseCloudMessaging = firebaseCloudMessaging;
            _hostingEnvironment = hostingEnvironment;
            _mangadexUploader = mangadexUploader;
            _naniWebContext = naniWebContext;
            _redditPoster = redditPoster;
            _settingsKeeper = settingsKeeper;
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

                var siteName = _settingsKeeper.GetSetting("SiteName").Value;
                var chapterUrl = $"{_settingsKeeper.GetSetting("SiteUrl").Value}{Url.Action("Project", "Home", new {urlSlug = series.UrlSlug, chapterNumber = chapter.ChapterNumber})}";
                var chapterDownloadUrl = $"{_settingsKeeper.GetSetting("SiteUrl").Value}{Url.Action("Download", "Home", new {urlSlug = series.UrlSlug, chapterNumber = chapter.ChapterNumber})}";
                var iconUrl = $"{_settingsKeeper.GetSetting("SiteUrl").Value}/assets/icon.png";
                var tasks = new Task[4];

                tasks[0] = Task.Run(async () =>
                {
                    var mangadexSeries = await _naniWebContext.MangadexSeries.SingleOrDefaultAsync(srs => srs.SeriesId == series.Id);

                    MangadexChapter mangadexChapter = null;

                    if (chapterAdd.UploadToMangadex && mangadexSeries.MangadexId > 0)
                        using (var stream = System.IO.File.OpenRead(pagesZip))
                        {
                            mangadexChapter = await _mangadexUploader.UploadChapter(chapter, mangadexSeries, stream);
                        }

                    if (mangadexChapter == null)
                        mangadexChapter = new MangadexChapter
                        {
                            Chapter = chapter,
                            ChapterId = chapter.Id,
                            MangadexId = 0
                        };

                    await _naniWebContext.MangadexChapters.AddAsync(mangadexChapter);
                    await _naniWebContext.SaveChangesAsync();
                });
                tasks[1] = Task.Run(async () =>
                {
                    await _firebaseCloudMessaging.SendNotification($"A new release is available at {siteName}!", $"New chapter of {series.Name} is available at {siteName}!", chapterUrl, iconUrl, $"series_{series.Id}");
                });
                tasks[2] = Task.Run(async () =>
                {
                    if (chapterAdd.AnnounceOnDiscord)
                        await _discordBot.SendMessage($"@everyone **{series.Name}** - Chapter {chapter.ChapterNumber} is out!{Environment.NewLine}Read it here: {chapterUrl}{Environment.NewLine}Download it here: {chapterDownloadUrl}");
                });
                tasks[3] = Task.Run(async () =>
                {
                    if (chapterAdd.AnnounceOnReddit)
                        await _redditPoster.PostLink("/r/manga", $"[DISC] {series.Name} - Chapter {chapter.ChapterNumber}", chapterUrl, chapterAdd.RedditNsfw);
                });

                await Task.WhenAll(tasks);
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
                var chapter = await _naniWebContext.Chapters.Include(chp => chp.MangadexInfo).SingleAsync(chp => chp.Id == chapterEdit.ChapterId);
                var mangadexSeries = await _naniWebContext.MangadexSeries.SingleAsync(mgdx => mgdx.SeriesId == chapter.SeriesId);
                chapter.Volume = chapterEdit.Volume ?? 0;
                chapter.ChapterNumber = chapterEdit.ChapterNumber;
                chapter.Name = chapterEdit.Name ?? string.Empty;
                chapter.MangadexInfo.MangadexId = chapterEdit.MangadexId ?? 0;

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
                    var pageList = tempPages.EnumerateFiles().Where(fl => fl.Extension == ".png").OrderBy(fl => fl.Name).ToList();
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

                    if (chapterEdit.UploadToMangadex && chapter.MangadexInfo.MangadexId > 0)
                        using (var stream = System.IO.File.OpenRead(pagesZip))
                        {
                            await _mangadexUploader.UpdateChapter(chapter, mangadexSeries, chapter.MangadexInfo, stream);
                        }

                    temp.Delete(true);
                }
                else if (chapterEdit.UploadToMangadex)
                {
                    await _mangadexUploader.UpdateChapterInfo(chapter, mangadexSeries, chapter.MangadexInfo);
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