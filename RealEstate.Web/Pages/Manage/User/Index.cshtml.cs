using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewComponents;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;

namespace RealEstate.Web.Pages.Manage.User
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : IndexPageModel
    {
        private readonly IUserService _userService;

        public IndexModel(
            IUserService userService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _userService = userService;
            PageTitle = sharedLocalizer[SharedResource.Users];
        }

        [BindProperty]
        public UserSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<UserViewModel> List { get; set; }

        public async Task OnGetAsync(string pageNo, string status, string userName, string userId, Role? userRole, bool deleted, string dateFrom, string dateTo, string creatorId)
        {
            SearchInput = new UserSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Username = userName,
                UserId = userId,
                Role = userRole,
                IncludeDeletedItems = deleted,
                CreatorId = creatorId,
                CreationDateFrom = dateFrom,
                CreationDateTo = dateTo
            };

            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            List = await _userService.ListAsync(SearchInput, false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.RouteDictionary());
        }

        public async Task<IActionResult> OnGetPageAsync([FromQuery] string json) =>
            await this.OnGetPageHandlerAsync<UserSearchViewModel, UserViewModel>(json,
                model => _userService.ListAsync(model),
                typeof(UserPageViewComponent));
    }
}