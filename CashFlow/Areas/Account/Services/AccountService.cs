using System;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using CashFlow.Common.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace CashFlow.Areas.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountService(UserManager<AppUser> userManager, IEmailSender emailSender, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
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
            string code = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));
            string url = $"https://localhost:44322/Account/Account/ConfirmEmail?userId={user.Id}&code={code}";
            string subject = "Подтверждение регистрации";
            string message = $"Подтвердите регистрацию, перейдя по ссылке: <a href='{url}'>link</a>";            
            
            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        public async Task SendRecoveryPasswordEmail(AppUser user)
        {
            string code = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
            string url = $"https://localhost:44322/Account/Account/RecoveyPassword?userId={user.Id}&code={code}";
            string subject = "Восстановление пароля";
            string message = $"Для восстановления пароля перейдите по ссылке: <a href='{url}'>link</a>";

            await _emailSender.SendEmailAsync(user.Email, subject,
                message);
        }

        public async Task<SignInResult> Login(AppUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public async Task Logout() => await _signInManager.SignOutAsync();
        public async Task<IdentityResult> ResetPassword(AppUser user, string code, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, code, newPassword);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string code)
        {
            return await _userManager.ConfirmEmailAsync(user, code);
        }
    }
}