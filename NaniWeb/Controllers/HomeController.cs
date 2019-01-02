using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NaniWeb.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
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
        public IActionResult Project(string urlSlug, decimal chapterNumber)
        {
            return null;
        }

        public IActionResult Projects()
        {
            return null;
        }
    }
}