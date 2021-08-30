using System;
using System.Collections.Generic;
using CashFlow.Areas.Account;
using CashFlow.Models;

namespace CashFlow.Areas.Users.ViewModels
{
    public class IndexViewModel
    {
        public List<AppUser> Users { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public string Sort { get; set; }
        public string Filter { get; set; }
    }
}