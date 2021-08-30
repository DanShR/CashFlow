using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace CashFlow.Areas.Account.Services
{
    public class RoleInitializer
    {
        private static readonly string AdminRoleName = "admin";
        private static readonly string UserRoleName = "user";
        private static readonly string AdminEmail = "dshulakov@gmail.com";
        private static readonly string AdminPassword = "123";
        private static Random random = new Random();

        public static async Task InitializeAsync(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            List<AppUser> users = userManager.Users.ToList();

            if (users.Count != 0)
            {
                await CreateUsers(userManager);
                return;
            }

            
            if (await roleManager.FindByNameAsync(AdminRoleName) == null)
                await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
            
            if (await roleManager.FindByNameAsync(UserRoleName) == null)
                await roleManager.CreateAsync(new IdentityRole(UserRoleName));
            
            AppUser adminUser = new AppUser() { Email = AdminEmail, UserName = AdminEmail, Name = AdminEmail, EmailConfirmed = true};
            await userManager.CreateAsync(adminUser, AdminPassword);
            await userManager.AddToRoleAsync(adminUser, AdminRoleName);
        }

        public static async Task CreateUsers(UserManager<AppUser> userManager)
        {
            var users = userManager.Users;
          
            if (users.Count() < 300)
            {
                for (int i = users.Count(); i < 350; i++)
                {
                    string email = "Test" + i + "@test.com";
                    string userName = email;
                    string name = RandomString();
                    AppUser adminUser = new AppUser() { Email = email, UserName = email, Name = name, EmailConfirmed = true };
                    await userManager.CreateAsync(adminUser, AdminPassword);
                    await userManager.AddToRoleAsync(adminUser, UserRoleName);
                }
            }

        }

        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}