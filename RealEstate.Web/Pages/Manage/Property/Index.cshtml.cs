using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Property
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly IPropertyService _propertyService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IPropertyService propertyService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _propertyService = propertyService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public PropertySearchViewModel SearchInput { get; set; }

        public PaginationViewModel<PropertyViewModel> List { get; set; }

        public string Status { get; set; }

        public string PageTitle => _localizer[SharedResource.Properties];

        public async Task OnGetAsync(string pageNo, string status, string propertyId, string propertyStreet, string propertyDistrict, string propertyCategory, string propertyOwner, string propertyOwnerMobile, bool deleted, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new PropertySearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Id = propertyId,
                District = propertyDistrict,
                Category = propertyCategory,
                Street = propertyStreet,
                Owner = propertyOwner,
                OwnerMobile = propertyOwnerMobile,
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _propertyService.PropertyListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}