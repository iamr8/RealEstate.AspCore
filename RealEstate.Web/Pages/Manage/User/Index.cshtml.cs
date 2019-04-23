using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Search;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.User
{
    [Authorize(Roles = "Admin,SuperAdmin")]
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
        public string PageTitle => _localizer["Users"];

        public async Task OnGetAsync(string pageNo, string userName, string userId, Role? userRole)
        {
            SearchInput = new UserSearchViewModel
            {
                PageNo = pageNo.FixPageNumber(),
                Username = userName,
                UserId = userId,
                Role = userRole
            };

            List = await _userService.ListAsync(SearchInput).ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(), SearchInput.GetSearchParameters());
        }
    }
}