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

namespace RealEstate.Web.Pages.Manage.Payment
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
        public PaymentSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<PaymentViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Payments"];

        public async Task OnGetAsync(string pageNo)
        {
            SearchInput = new PaymentSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
            };

            List = await _paymentService.PaymentListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}