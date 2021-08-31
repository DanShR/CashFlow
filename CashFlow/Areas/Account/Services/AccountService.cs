using System;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using CashFlow.Common.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace CashFlow.Areas.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountService(UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IdentityResult> AddUser(string email, string name, string password)
        {
            AppUser user = new AppUser
            {
                Email = email,
                UserName = email,
                Name = name,
                IsActive = true,
                RegisterDate = DateTime.Now
            };

            IdentityResult result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task SendConfirmEmail(AppUser user)
        {
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $"https://localhost:44322/Account/Account/ConfirmEmail?userId={user.Id}&code={code}";
            string subject = "Подтверждение регистрации";
            string message = $"Подтвердите регистрацию, перейдя по ссылке: <a href='{url}'>link</a>";

            //await _accountService.SendConfirmEmail(user.Email, callbackUrl);
            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        public async Task SendRecoveryPasswordEmail(string email, string callbackUrl)
        {
            await _emailSender.SendEmailAsync(email, "Восстановление пароля",
                $"Для восстановления пароля перейдите по ссылке: <a href='{callbackUrl}'>link</a>");
        }
    }
}