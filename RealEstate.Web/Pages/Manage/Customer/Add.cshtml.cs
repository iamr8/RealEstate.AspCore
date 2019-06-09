using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Customer
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : PageModel
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
        public CustomerInputViewModel NewCustomer { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public string Status { get; set; }

        [TempData]
        public string PassJson { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            CustomerInputViewModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                model = await _customerService.CustomerInputAsync(id).ConfigureAwait(false);
            }

            NewCustomer = !string.IsNullOrEmpty(id)
                ? model.UsePassModelForEdit(PassJson)
                : model.UsePassModelForAdd(PassJson);
            PassJson = default;
            Status = !string.IsNullOrEmpty(status) ? status : null;
            PageTitle = _localizer[(model == null ? "New" : "Edit") + GetType().Namespaces().Last()];

            if (!string.IsNullOrEmpty(id) && model == null)
                return RedirectToPage(typeof(IndexModel).Page());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _customerService.CustomerAddOrUpdateAsync(NewCustomer, !NewCustomer.IsNew, true)
            ).ConfigureAwait(false);

            PassJson = NewCustomer.SerializePassModel();
            return RedirectToPage(status != StatusEnum.Success
                ? typeof(AddModel).Page()
                : typeof(IndexModel).Page(), new
                {
                    status = message,
                    id = status != StatusEnum.Success ? NewCustomer?.Id : null
                });
        }
    }
}