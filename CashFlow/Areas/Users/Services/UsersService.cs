using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashFlow.Areas.Account;
using CashFlow.Areas.Users.Model;
using Microsoft.AspNetCore.Identity;

namespace CashFlow.Areas.Users.Services
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IEnumerable<AppUser> GetAllUsers()
        {
            return _userManager.Users;
        }

        public IEnumerable<AppUser> GetUsersWithFilter(string filter)
        {
            IEnumerable<AppUser> allUsers = GetAllUsers();
            IEnumerable<AppUser> users;

            if (!String.IsNullOrWhiteSpace(filter))
            {
                users = allUsers.Where(user => user.Email.ToLower().Contains(filter.ToLower())
                                                || user.Name.ToLower().Contains(filter.ToLower()));
            }
            else
            {
                users = allUsers;
            }
            return users;
        }


        public IEnumerable<AppUser> GetUsersPage(IEnumerable<AppUser> source, int page, int pageSize, SortType sortType)
        {
            IEnumerable<AppUser> users;

            switch (sortType)
            {
                case SortType.Email:
                    users = source.OrderBy(user => user.Email).ToList();
                    break;
                case SortType.EmailDesc:
                    users = source.OrderByDescending(user => user.Email).ToList();
                    break;
                case SortType.Name:
                    users = source.OrderBy(user => user.Name);
                    break;
                case SortType.NameDesc:
                    users = source.OrderByDescending(user => user.Name).ToList();
                    break;
                default:
                    users = source.OrderBy(user => user.Email).ToList();
                    break;
            }

            IEnumerable<AppUser> items = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return items;
        }

        public async Task<AppUser> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task ChangeUserRoles(AppUser user, IEnumerable<string> roles)
        {
            IEnumerable<string> userRoles = await GetUserRolesAsync(user);
            IEnumerable<IdentityRole> allRoles = GetAllRoles();

            IEnumerable<string> addedRoles = roles.Except(userRoles);
            IEnumerable<string> removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);
        }

        public async Task<AppUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}