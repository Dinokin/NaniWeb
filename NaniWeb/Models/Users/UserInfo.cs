using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NaniWeb.Models.Users
{
    public class Users
    {
        public List<IdentityUser<int>> UserList { get; set; }
    }
}