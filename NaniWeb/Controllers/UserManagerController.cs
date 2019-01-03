using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NaniWeb.Controllers
{
    public class UserManagerController : Controller
    {
        private readonly UserManager<IdentityUser<int>> _userManager;

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> List()
        {            
            return View("UserList", await _userManager.Users.ToListAsync());
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditRole(int userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.RemoveFromRolesAsync(user, new[] {"Administrator", "Moderator", "Uploader"});
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("List");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            return null;
        }
    }
}