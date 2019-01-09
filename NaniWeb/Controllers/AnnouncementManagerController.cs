using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Data;
using NaniWeb.Models.Announcement;
using NaniWeb.Others;

namespace NaniWeb.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class AnnouncementManagerController : Controller
    {
        private readonly NaniWebContext _naniWebContext;

        public AnnouncementManagerController(NaniWebContext naniWebContext)
        {
            _naniWebContext = naniWebContext;
        }

        public async Task<IActionResult> List()
        {
            var announcements = await _naniWebContext.Announcements.OrderByDescending(ann => ann.PostDate).ToArrayAsync();

            ViewData["Announcements"] = announcements;

            return View("AnnouncementList");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("AddAnnouncement");
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAnnouncement addAnnouncement)
        {
            if (ModelState.IsValid)
            {
                var announcement = new Announcement
                {
                    Title = addAnnouncement.Title,
                    Content = addAnnouncement.Content,
                    PostDate = DateTime.UtcNow,
                    UrlSlug = Utils.GenerateSlug(addAnnouncement.Title)
                };

                await _naniWebContext.Announcements.AddAsync(announcement);
                await _naniWebContext.SaveChangesAsync();

                return RedirectToAction("List");
            }

            TempData["Error"] = true;

            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _naniWebContext.Announcements.SingleAsync(ann => ann.Id == id);
            var model = new EditAnnouncement
            {
                AnnouncementId = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content
            };

            return View("EditAnnouncement", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAnnouncement editAnnouncement)
        {
            if (ModelState.IsValid)
            {
                var announcement = await _naniWebContext.Announcements.SingleAsync(ann => ann.Id == editAnnouncement.AnnouncementId);
                announcement.Title = editAnnouncement.Title;
                announcement.Content = editAnnouncement.Content;
                announcement.UrlSlug = Utils.GenerateSlug(editAnnouncement.Title);

                _naniWebContext.Announcements.Update(announcement);
                await _naniWebContext.SaveChangesAsync();

                return RedirectToAction("List");
            }

            TempData["Error"] = true;

            return RedirectToAction("Edit", new {id = editAnnouncement.AnnouncementId});
        }

        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _naniWebContext.Announcements.SingleAsync(ann => ann.Id == id);
            _naniWebContext.Announcements.Remove(announcement);
            await _naniWebContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}