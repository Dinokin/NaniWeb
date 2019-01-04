using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaniWeb.Models.Users;

namespace NaniWeb.Controllers
{
    public class UserManagerController : Controller
    {
        private readonly UserManager<IdentityUser<int>> _userManager;

        public UserManagerController(UserManager<IdentityUser<int>> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> List()
        {
            var model = new List<User>();
            
            foreach (var identityUser in _userManager.Users)
            {
                var roles = await _userManager.GetRolesAsync(identityUser);
                
                model.Add(new User
                {
                    Id = identityUser.Id,
                    Username = identityUser.UserName,
                    Role = roles.Count > 0 ? roles[0] : "None"
                });
            }
            
            return View("UserList", model);
        }

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.DeleteAsync(user);
            
            return RedirectToAction("List");
        }
    }
}