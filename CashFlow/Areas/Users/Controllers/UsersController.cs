using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashFlow.Areas.Account;
using CashFlow.Areas.Users.ViewModels;
using CashFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Areas.User.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET
        public async Task<IActionResult> Index(int page = 1, string sort = "email", string filter = "")
        {
            IEnumerable<AppUser> allUsers = _userManager.Users;
            IEnumerable<AppUser> users;
            IEnumerable<AppUser> source;

            if (filter != null)
            {
                source = allUsers.Where(user => user.Email.ToLower().Contains(filter.ToLower()) 
                                                || user.Name.ToLower().Contains(filter.ToLower()));
            }
            else
            {
                source = allUsers;
            }

            switch (sort)
            {
                case "email":
                    users = source.OrderBy(user => user.Email).ToList();
                    break;
                case "emaildesc":
                    users = source.OrderByDescending(user => user.Email).ToList();
                    break;
                case "name":
                    users = source.OrderBy(user => user.Name);
                    break;
                case "namedesc":
                    users = source.OrderByDescending(user => user.Name).ToList();
                    break;
                default:
                    users = source.OrderBy(user => user.Email).ToList(); 
                    break;

            }
            int pageSize = 20;   

            int count = users.Count();
            List<AppUser> items = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            List<UserViewModel> userViewModels = items.Select(user => new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            }).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Users = items,
                Sort = sort,
                Filter = filter
            };
            return View(viewModel);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            List<IdentityRole> allRoles = _roleManager.Roles.ToList();

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
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                
                var allRoles = _roleManager.Roles.ToList();
                
                var addedRoles = roles.Except(userRoles);
                
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("Index", "Users");
            }

            return NotFound();
        }
    }
}