using Microsoft.AspNetCore.Authorization;
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

namespace RealEstate.Web.Pages.Manage.User
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [NavBarHelper(typeof(IndexModel))]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(
            IUserService userService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _userService = userService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public UserSearchViewModel SearchInput { get; set; }

        public PaginationViewModel<UserViewModel> List { get; set; }

        [ViewData]
        public string PageTitle => _localizer[SharedResource.Users];

        public StatusEnum Status { get; set; }

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

            Status = !string.IsNullOrEmpty(status) && int.TryParse(status, out var statusInt)
                ? (StatusEnum)statusInt
                : StatusEnum.Ready;
            List = await _userService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}