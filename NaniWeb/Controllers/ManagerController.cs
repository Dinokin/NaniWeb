using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NaniWeb.Controllers
{
    [Authorize(Roles = "Administrator, Moderator, Uploader")]
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Add", "ChapterManager");
        }
    }
}