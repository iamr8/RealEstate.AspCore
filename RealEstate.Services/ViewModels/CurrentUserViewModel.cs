using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.ViewModels.Json;
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
        public string EmployeeId { get; set; }
        public List<DivisionJsonViewModel> EmployeeDivisions { get; set; }
        public List<CategoryJsonViewModel> UserItemCategories { get; set; }
        public List<CategoryJsonViewModel> UserPropertyCategories { get; set; }
    }
}