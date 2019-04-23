using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RealEstate.Web.Pages.Manage.Sms
{
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