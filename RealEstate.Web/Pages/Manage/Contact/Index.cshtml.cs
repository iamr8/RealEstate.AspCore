using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Contact
{
    public class IndexModel : PageModel
    {
        private readonly IContactService _contactService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IContactService contactService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _contactService = contactService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ContactSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<ContactViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer["Contacts"];

        public async Task OnGetAsync(string pageNo, string contactName, string contactId, string contactAddress, string contactPhone, string contactMobile)
        {
            SearchInput = new ContactSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Name = contactName,
                Id = contactId,
                Address = contactAddress,
                Phone = contactPhone,
                Mobile = contactMobile
            };

            List = await _contactService.ContactListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(Contact.IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}