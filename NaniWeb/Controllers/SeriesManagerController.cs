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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly NaniWebContext _naniWebContext;

        public SeriesManagerController(IHostingEnvironment hostingEnvironment, NaniWebContext naniWebContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _naniWebContext = naniWebContext;
        }

        [Authorize(Roles = "Administrator, Moderator, Uploader")]
        public async Task<IActionResult> List()
        {
            ViewData["Series"] = await _naniWebContext.Series.OrderBy(key => key.Id).ToListAsync();

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
                        MangadexId = addSeries.MangadexId ?? 0
                    }
                };

                await _naniWebContext.Series.AddAsync(series);
                await _naniWebContext.SaveChangesAsync();

                var path = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}{series.Id}.png";

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
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == id);
            var mangadex = await _naniWebContext.MangadexSeries.SingleOrDefaultAsync(srs => srs.Series == series);
            var model = new EditSeries
            {
                SeriesId = series.Id,
                Name = series.Name,
                Author = series.Author,
                Artist = series.Artist,
                Synopsis = series.Synopsis,
                Type = series.Type,
                Status = series.Status,
                MangadexId = mangadex.MangadexId
            };

            return View("EditSeries", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditSeries editSeries)
        {
            if (ModelState.IsValid)
            {
                var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == editSeries.SeriesId);
                var mangadex = await _naniWebContext.MangadexSeries.SingleOrDefaultAsync(srs => srs.Series == series);

                series.Name = editSeries.Name;
                series.Author = editSeries.Author;
                series.Artist = editSeries.Artist;
                series.Synopsis = editSeries.Synopsis;
                series.Type = editSeries.Type;
                series.Status = editSeries.Status;
                series.UrlSlug = Utils.GenerateSlug(editSeries.Name);
                mangadex.MangadexId = editSeries.MangadexId ?? 0;
                series.MangadexInfo = mangadex;

                _naniWebContext.Series.Update(series);
                await _naniWebContext.SaveChangesAsync();

                if (editSeries.Cover != null)
                {
                    var path = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}{series.Id}.png";

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
            var coversLocation = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}covers{Path.DirectorySeparatorChar}";
            var pagesLocation = $"{_hostingEnvironment.WebRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}";
            var series = await _naniWebContext.Series.SingleAsync(srs => srs.Id == id);
            var pages = _naniWebContext.Pages.Where(pg => pg.Chapter.SeriesId == id);

            System.IO.File.Delete($"{coversLocation}{series.Id}.png");

            foreach (var page in pages)
                System.IO.File.Delete($"{pagesLocation}{page.Id}.png");

            _naniWebContext.Series.Remove(series);
            await _naniWebContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}