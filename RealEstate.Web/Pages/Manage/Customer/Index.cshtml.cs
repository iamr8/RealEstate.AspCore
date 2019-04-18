using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Web.Pages.Manage.Customer
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            ICustomerService customerService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _customerService = customerService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public CustomerSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<CustomerViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Contacts"];

        public async Task OnGetAsync(string pageNo, string customerName, string customerId, string customerAddress, string customerPhone, string customerMobile)
        {
            SearchInput = new CustomerSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = customerName,
                Id = customerId,
                Address = customerAddress,
                Phone = customerPhone,
                Mobile = customerMobile
            };

            List = await _customerService.CustomerListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}