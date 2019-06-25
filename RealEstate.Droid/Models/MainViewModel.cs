using System.Collections.Generic;

namespace RealEstate.Droid.Models
{
    public class MainViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string MobileNumber { get; set; }

        public List<string> UserItemCategories { get; set; }

        public List<string> UserPropertyCategories { get; set; }

        public List<string> EmployeeDivisions { get; set; }
    }
}