using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NaniWeb.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserManagerController : Controller
    {
        private readonly UserManager<IdentityUser<int>> _userManager;

        public UserManagerController(UserManager<IdentityUser<int>> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> List()
        {
            var users = await _userManager.Users.OrderBy(user => user.UserName).ToArrayAsync();
            var userRoleArray = new Tuple<IdentityUser<int>, string>[_userManager.Users.Count()];

            for (var i = 0; i < users.Length; i++)
            {
                var roles = await _userManager.GetRolesAsync(users[i]);
                userRoleArray[i] = new Tuple<IdentityUser<int>, string>(users[i], roles.Count > 0 ? roles[0] : "None");
            }

            ViewData["Users"] = userRoleArray;

            return View("UserList");
        }

        public async Task<IActionResult> EditRole(int userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.RemoveFromRoleAsync(user, "Administrator");
            await _userManager.RemoveFromRoleAsync(user, "Moderator");
            await _userManager.RemoveFromRoleAsync(user, "Uploader");

            if (role != "None")
                await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.DeleteAsync(user);

            return RedirectToAction("List");
        }
    }
}