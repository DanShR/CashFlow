using System.ComponentModel.DataAnnotations;

namespace CashFlow.Areas.Account.ViewModels
{
    public class RecoveryPasswordViewModel
    {
        [Required] 
        public string Id { get; set; }

        [Required] 
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}