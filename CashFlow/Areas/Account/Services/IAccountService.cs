using System.Collections.Generic;
using System.Threading.Tasks;
using CashFlow.Areas.Account.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CashFlow.Areas.Account.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> AddUser(string email, string name, string password);
        Task SendConfirmEmail(AppUser user);
        Task<IdentityResult> ConfirmEmailAsync(AppUser user, string code);
        Task SendRecoveryPasswordEmail(string email, string callbackUrl);
        Task<SignInResult> Login(AppUser user, string password, bool isPersistent, bool lockoutOnFailure);
        Task 
    }
}