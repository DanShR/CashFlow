using System;
using Microsoft.AspNetCore.Identity;

namespace CashFlow.Areas.Account
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
    }

    
}