using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashFlow.Areas.Account;
using CashFlow.Areas.Users.Model;
using CashFlow.Areas.Users.Services;
using CashFlow.Areas.Users.ViewModels;
using CashFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Areas.User.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<IActionResult> Index(int page, string sort, string filter)
        {
            const int pageSize = 20;

            var pageValue = page > 0 ? page : 1;
            SortType sortValue;
            sortValue = Enum.TryParse(sort, true, out sortValue) ? sortValue : SortType.Email;
            
            string filterValue = String.IsNullOrWhiteSpace(filter) ? String.Empty : filter;
            IEnumerable<AppUser> usersSource = _usersService.GetUsersWithFilter(filterValue); 

            IEnumerable<AppUser> users = _usersService.GetUsersPage(usersSource, pageValue, pageSize, sortValue);
            
            PageViewModel pageViewModel = new PageViewModel(usersSource.Count(), pageValue, pageSize);

            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Users = users,
                Sort = sortValue,
                Filter = filter
            };
            return View(viewModel);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            AppUser user = await _usersService.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IEnumerable<string> userRoles = await _usersService.GetUserRolesAsync(user);
            IEnumerable<IdentityRole> allRoles = _usersService.GetAllRoles();

            EditRoleViewModel model = new EditRoleViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string userId, IEnumerable<string> roles)
        {
            AppUser user = await _usersService.FindByIdAsync(userId);
            if (user != null)
            {
                await _usersService.ChangeUserRoles(user, roles);
                return RedirectToAction("Index", "Users");
            }

            return NotFound();
        }
    }
}