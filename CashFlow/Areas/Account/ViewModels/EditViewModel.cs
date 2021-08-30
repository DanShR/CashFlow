using System;
using System.ComponentModel.DataAnnotations;

namespace CashFlow.Areas.Account.ViewModels
{
    public class EditViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime RegisterDate { get; set; }
    }
}