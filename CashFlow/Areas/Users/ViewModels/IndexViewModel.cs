using System;
using System.Collections.Generic;
using CashFlow.Areas.Account;
using CashFlow.Areas.Users.Model;
using CashFlow.Models;

namespace CashFlow.Areas.Users.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<AppUser> Users { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public SortType Sort { get; set; }
        public string Filter { get; set; }
    }
}