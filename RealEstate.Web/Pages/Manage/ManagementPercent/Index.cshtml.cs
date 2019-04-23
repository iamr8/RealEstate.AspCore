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

namespace RealEstate.Web.Pages.Manage.ManagementPercent
{
    public class IndexModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IPaymentService paymentService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _paymentService = paymentService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ManagementPercentSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<ManagementPercentViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["ManagementPercents"];

        public async Task OnGetAsync(string pageNo, string percent)
        {
            SearchInput = new ManagementPercentSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Percent = string.IsNullOrEmpty(percent)
                    ? (int?)null
                    : int.TryParse(percent, out var page)
                        ? page
                        : 0
            };

            List = await _paymentService.ManagementPercentListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}