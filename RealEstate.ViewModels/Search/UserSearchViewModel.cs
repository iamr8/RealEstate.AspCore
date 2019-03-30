using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Enums;
using RealEstate.Resources;

namespace RealEstate.ViewModels.Search
{
    public class UserSearchViewModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "LastName")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Role")]
        public Role? Role { get; set; }
    }
}