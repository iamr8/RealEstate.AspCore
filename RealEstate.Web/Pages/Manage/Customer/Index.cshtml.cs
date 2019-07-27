using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewComponents;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Web.Pages.Manage.Customer
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
    {
        private readonly ICustomerService _customerService;

        public IndexModel(
            ICustomerService customerService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _customerService = customerService;
            PageTitle = sharedLocalizer[SharedResource.Customers];
        }

        [BindProperty]
        public CustomerSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<CustomerViewModel> List { get; set; }

        public async Task OnGetAsync(string pageNo, string customerName, string customerId, string customerAddress, string customerPhone, string customerMobile,
            bool deleted, string dateFrom, string dateTo, string creatorId, string status)
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
                CreationDateTo = dateTo,
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _customerService.CustomerListAsync(SearchInput, false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }

        public async Task<IActionResult> OnGetPageAsync(CustomerSearchViewModel models)
        {
            var list = await _customerService.CustomerListAsync(models);
            return ViewComponent(typeof(CustomerPageViewComponent), new
            {
                models = list.Items
            });
        }
    }
}