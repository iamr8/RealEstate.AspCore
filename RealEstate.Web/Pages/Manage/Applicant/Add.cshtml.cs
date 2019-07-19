using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Applicant
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : AddPageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            ICustomerService customerService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _customerService = customerService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ApplicantInputViewModel NewApplicant { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _customerService.ApplicantInputAsync(identifier),
                typeof(Employee.IndexModel).Page(),
                true);
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _customerService.ApplicantAddOrUpdateAsync(NewApplicant, !NewApplicant.IsNew, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}