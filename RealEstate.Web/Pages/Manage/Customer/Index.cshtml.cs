using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;

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
        public string PageTitle => _localizer["Customers"];

        public async Task OnGetAsync(string pageNo, string customerName, string customerId, string customerAddress, string customerPhone, string customerMobile, bool deleted, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new CustomerSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = customerName,
                Id = customerId,
                Address = customerAddress,
                Phone = customerPhone,
                Mobile = customerMobile,
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            List = await _customerService.CustomerListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}