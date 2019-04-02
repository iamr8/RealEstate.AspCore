using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Extensions;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.ViewModels;
using RealEstate.ViewModels.Search;
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

        public int PageNo { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        public async Task OnGetAsync(string pageNo, string userName, string userFirst, string userLast,
            string userMobile, string userAddress, string password, string role, string userId)
        {
            SearchInput = new UserSearchViewModel
            {
                Address = userAddress,
                Mobile = userMobile,
                Username = userName,
                FirstName = userFirst,
                LastName = userLast,
                Password = password,
                Role = (Role?)(string.IsNullOrEmpty(role)
                    ? (System.Enum)null
                    : role.To<Role>())
            };

            PageTitle = _localizer["Users"];
            var pg = string.IsNullOrEmpty(pageNo) ? 1 : int.TryParse(pageNo, out var page) ? page : 1;
            PageNo = pg;
            List = await _userService
                .ListAsync(pg, userName, userFirst, userLast, userMobile, userAddress, password, role, userId)
                .ConfigureAwait(false);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(typeof(IndexModel).Page(),
                new
                {
                    userName = SearchInput.Username,
                    userFirst = SearchInput.FirstName,
                    userLast = SearchInput.LastName,
                    userMobile = SearchInput.Mobile,
                    userAddress = SearchInput.Address,
                    password = SearchInput.Password,
                    role = SearchInput.Role
                });
        }
    }
}