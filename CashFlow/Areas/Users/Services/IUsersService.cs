using System.Collections.Generic;
using System.Threading.Tasks;
using CashFlow.Areas.Account;
using CashFlow.Areas.Users.Model;
using Microsoft.AspNetCore.Identity;

namespace CashFlow.Areas.Users.Services
{
    public interface IUsersService
    {
        IEnumerable<AppUser> GetAllUsers();
        IEnumerable<AppUser> GetUsersPage(IEnumerable<AppUser> source, int page, int pageSize, SortType sortType);
        Task<AppUser> FindByIdAsync(string id);
        Task<AppUser> FindByEmailAsync(string email);
        Task<IEnumerable<string>> GetUserRolesAsync(AppUser user);
        IEnumerable<IdentityRole> GetAllRoles();
        Task ChangeUserRoles(AppUser user, IEnumerable<string> roles);
        IEnumerable<AppUser> GetUsersWithFilter(string filter);
    }
}