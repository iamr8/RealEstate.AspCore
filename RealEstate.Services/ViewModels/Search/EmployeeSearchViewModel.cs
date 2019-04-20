using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.ViewModels.Search
{
    public class EmployeeSearchViewModel : BaseSearchModel
    {
        [Display(ResourceType = typeof(SharedResource), Name = "FirstName")]
        [SearchParameter("employeeFirstName")]
        public string FirstName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "LastName")]
        [SearchParameter("employeeLastName")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Mobile")]
        [SearchParameter("employeeMobile")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "PhoneNumber")]
        [SearchParameter("employeePhone")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Address")]
        [SearchParameter("employeeAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(SharedResource), Name = "Id")]
        [SearchParameter("employeeId")]
        public string Id { get; set; }

        [SearchParameter("userId")]
        public string UserId { get; set; }

        [SearchParameter("divisionId")]
        public string DivisionId { get; set; }
    }
}