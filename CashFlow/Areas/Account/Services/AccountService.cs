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

        public async Task<IdentityResult> AddUser(AppUser user, string password)
        {
            IdentityResult result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task SendConfirmEmail(string email, string callbackUrl)
        {
            
            await _emailSender.SendEmailAsync(email, "Подтверждение регистрации",
                $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
        }

        public async Task SendRecoveryPasswordEmail(string email, string callbackUrl)
        {
            await _emailSender.SendEmailAsync(email, "Восстановление пароля",
                $"Для восстановления пароля перейдите по ссылке: <a href='{callbackUrl}'>link</a>");
        }
    }
}