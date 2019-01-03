using Microsoft.AspNetCore.Mvc;

namespace NaniWeb.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View("Profile");
        }

        public IActionResult ChangeEmail()
        {
            return null;
        }

        public IActionResult ChangePassword()
        {
            return null;
        }

        public IActionResult DeleteAccount()
        {
            return null;
        }
    }
}