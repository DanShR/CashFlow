using System.Threading.Tasks;

namespace CashFlow.Common.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}