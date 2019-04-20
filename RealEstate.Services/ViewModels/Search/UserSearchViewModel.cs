using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class UserSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "Id")]
        [SearchParameter("userId")]
        public string UserId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        [SearchParameter("userName")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Role")]
        [SearchParameter("userRole")]
        public Role? Role { get; set; }
    }
}