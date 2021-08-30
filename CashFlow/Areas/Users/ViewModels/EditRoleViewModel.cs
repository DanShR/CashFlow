using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CashFlow.Areas.Users.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
        public IEnumerable<IdentityRole> AllRoles { get; set; }
    }
}