using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;
using NaniWeb.Models.Series;
using NaniWeb.Others;

namespace NaniWeb.Controllers
{
    public class SeriesManagerController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly NaniWebContext _naniWebContext;

        public SeriesManagerController(IWebHostEnvironment webHostEnvironment, NaniWebContext naniWebContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _naniWebContext = naniWebContext;
        }

        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        public async Task<IActionResult> List()
        {
            var series = await _naniWebContext.Series.OrderBy(key => key.Id).ToArrayAsync();

            ViewData["Series"] = series;

            return View("SeriesList");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Add()
        {
            return View("AddSeries");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Add(AddSeries addSeries)
        {
            if (ModelState.IsValid)
            {
                var series = new Series
                {
                    Name = addSeries.Name,
                    Author = addSeries.Author,
                    Artist = addSeries.Artist,
                    Synopsis = addSeries.Synopsis,
                    Type = addSeries.Type,
                    Status = addSeries.Status,
                    UrlSlug = Utils.GenerateSlug(addSeries.Name),
                    MangadexInfo = new MangadexSeries
                    {
                        MangadexId = addSeries.MangadexId ?? 0,
                        DisplayLink = addSeries.DisplayMangadexLink
                    }
                };

                await _naniWebContext.Series.AddAsync(series);
                await _naniWebContext.SaveChangesAsync();

                var path = $"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}{series.Id}.png";
                Directory.CreateDirectory(path.Replace($"{Path.DirectorySeparatorChar}{series.Id}.png", string.Empty));

                using (var file = System.IO.File.Create(path))
                {
                    await addSeries.Cover.CopyToAsync(file);
                }

                var fileInfo = new FileInfo(path);
                var destination = fileInfo.Directory;

                Utils.ResizeImage(fileInfo, destination, $"{series.Id}_small", 209, 300);
                Utils.ResizeImage(fileInfo, destination, $"{series.Id}_smaller", 209 / 2, 300 / 2);

                return RedirectToAction("List");
            }

            TempData["Error"] = true;

            return RedirectToAction("Add");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            var series = await _naniWebContext.Series.Include(srs => srs.MangadexInfo).SingleAsync(srs => srs.Id == id);

            var model = new EditSeries
            {
                SeriesId = series.Id,
                Name = series.Name,
                Author = series.Author,
                Artist = series.Artist,
                Synopsis = series.Synopsis,
                Type = series.Type,
                Status = series.Status,
                MangadexId = series.MangadexInfo.MangadexId,
                DisplayMangadexLink = series.MangadexInfo.DisplayLink
            };

            return View("EditSeries", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditSeries editSeries)
        {
            if (ModelState.IsValid)
            {
                var series = await _naniWebContext.Series.Include(srs => srs.MangadexInfo).SingleAsync(srs => srs.Id == editSeries.SeriesId);

                series.Name = editSeries.Name;
                series.Author = editSeries.Author;
                series.Artist = editSeries.Artist;
                series.Synopsis = editSeries.Synopsis;
                series.Type = editSeries.Type;
                series.Status = editSeries.Status;
                series.UrlSlug = Utils.GenerateSlug(editSeries.Name);
                series.MangadexInfo.MangadexId = editSeries.MangadexId ?? 0;
                series.MangadexInfo.DisplayLink = editSeries.DisplayMangadexLink;

                _naniWebContext.Series.Update(series);
                await _naniWebContext.SaveChangesAsync();

                if (editSeries.Cover != null)
                {
                    var path = $"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}{series.Id}.png";

                    using (var file = System.IO.File.Create(path))
                    {
                        await editSeries.Cover.CopyToAsync(file);
                    }

                    var fileInfo = new FileInfo(path);
                    var destination = fileInfo.Directory;

                    Utils.ResizeImage(fileInfo, destination, $"{series.Id}_small", 209, 300);
                    Utils.ResizeImage(fileInfo, destination, $"{series.Id}_smaller", 209 / 2, 300 / 2);
                }

                return RedirectToAction("List");
            }

            TempData["Error"] = true;

            return RedirectToAction("Edit");
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int id)
        {
            var coversLocation = $"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}";
            var pagesLocation = $"{_webHostEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == id);
            var chapters = _naniWebContext.Chapters.Where(chp => chp.Series == series).Include(chp => chp.Pages);
            var downloadsDir = Utils.CurrentDirectory.CreateSubdirectory("Downloads");

            System.IO.File.Delete($"{coversLocation}{series.Id}.png");
            System.IO.File.Delete($"{coversLocation}{series.Id}_small.png");
            System.IO.File.Delete($"{coversLocation}{series.Id}_smaller.png");

            foreach (var chapter in chapters)
            {
                var file = $"{downloadsDir.FullName}{Path.DirectorySeparatorChar}{chapter.Id}.zip";

                foreach (var page in chapter.Pages)
                    System.IO.File.Delete($"{pagesLocation}{page.Id}.png");

                System.IO.File.Delete(file);
            }

            _naniWebContext.Series.Remove(series);
            await _naniWebContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}