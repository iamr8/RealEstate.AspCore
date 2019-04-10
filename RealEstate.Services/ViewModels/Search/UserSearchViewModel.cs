using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Database.Tables;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class UserSearchViewModel : BaseSearchModel
    {
        private readonly User _model;

        public UserSearchViewModel()
        {
        }

        public UserSearchViewModel(User model, int pageNo)
        {
            if (model == null)
                return;

            _model = model;
            PageNo = pageNo;
            UserId = _model.Id;
            FirstName = _model.FirstName;
            LastName = _model.LastName;
            Username = _model.Username;
            Password = _model.Password;
            Mobile = _model.Mobile;
            Address = _model.Address;
            Role = _model.Role;
        }

        [Display(ResourceType = typeof(SharedResource), Name = "Id")]
        [SearchParameter("userId")]
        public string UserId { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [SearchParameter("userFirst")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "LastName")]
        [SearchParameter("userLast")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Username")]
        [SearchParameter("userName")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Password")]
        [SearchParameter("password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [SearchParameter("userMobile")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [SearchParameter("userAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Role")]
        [SearchParameter("role")]
        public Role? Role { get; set; }
    }
}