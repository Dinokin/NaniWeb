using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NaniWeb.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SettingsManagerController : Controller
    {
        public IActionResult General()
        {
            return null;
        }

        public IActionResult Email()
        {
            return null;
        }

        public IActionResult Discord()
        {
            return null;
        }

        public IActionResult Mangadex()
        {
            return null;
        }

        public IActionResult GoogleAnalytics()
        {
            return null;
        }

        public IActionResult Fcm()
        {
            return null;
        }

        public IActionResult Disqus()
        {
            return null;
        }

        public IActionResult Facebook()
        {
            return null;
        }
    }
}