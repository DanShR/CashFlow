using System.Collections.Generic;
using System.Threading.Tasks;
using CashFlow.Areas.Account.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CashFlow.Areas.Account.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> AddUser(AppUser user, string password);
        Task SendConfirmEmail(string email, string callbackUrl);
        Task SendRecoveryPasswordEmail(string email, string callbackUrl);
    }
}