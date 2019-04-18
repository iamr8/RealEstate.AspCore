using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class CurrentUserViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Mobile { get; set; }
        public string EncryptedPassword { get; set; }
        public string FirstName { get; set; }
        public Role Role { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public List<UserItemCategoryViewModel> UserItemCategories { get; set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; set; }
    }
}