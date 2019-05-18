using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Sms
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly ISmsService _smsService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            ISmsService smsService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _smsService = smsService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public SmsSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<SmsViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Smses"];

        public async Task OnGetAsync(string pageNo)
        {
            SearchInput = new SmsSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
            };

            List = await _smsService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}