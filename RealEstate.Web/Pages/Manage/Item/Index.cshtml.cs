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

namespace RealEstate.Web.Pages.Manage.Item
{
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
    {
        private readonly IItemService _itemService;

        public IndexModel(
            IItemService itemService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _itemService = itemService;
            PageTitle = sharedLocalizer[SharedResource.Items];
        }

        [BindProperty]
        public ItemSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<ItemViewModel> List { get; set; }

        public async Task OnGetAsync(string pageNo, string status, string id, string street, string itemCategory, string ownerName, string customerId,
            string features, string facilities, string propertyCategory, string district, bool deleted, string ownerMobile, string dateFrom, string dateTo, string creatorId, string hasFeature, bool? removeDuplicates, bool? negotiable, bool? hasPicture)
        {
            SearchInput = new ItemSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Street = street,
                ItemCategory = itemCategory,
                Owner = ownerName,
                ItemId = id,
                CustomerId = customerId,
                FeaturesJson = features,
                FacilitiesJson = facilities,
                PropertyCategory = propertyCategory,
                District = district,
                IncludeDeletedItems = deleted,
                OwnerMobile = ownerMobile,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo,
                CreatorId = creatorId,
                HasFeature = hasFeature,
                IsNegotiable = negotiable != null && negotiable == true,
                HasPicture = hasPicture != null && hasPicture == true
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _itemService.ItemListAsync(SearchInput, false);
        }

        public IActionResult OnPost() =>
            RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());

        public async Task<IActionResult> OnGetPageAsync([FromQuery] string json) =>
            await this.OnGetPageHandlerAsync<ItemSearchViewModel, ItemViewModel>(json,
                model => _itemService.ItemListAsync(model),
                typeof(ItemPageViewComponent));
    }
}