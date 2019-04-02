using Microsoft.AspNetCore.Mvc.RazorPages;
using RealEstate.Base;
using RealEstate.Services;
using RealEstate.ViewModels;

namespace RealEstate.Web.Pages.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly IUserService _userService;

        public IndexModel(
            IItemService itemService,
            IUserService userService
            )
        {
            _itemService = itemService;
            _userService = userService;
        }

        public PaginationViewModel<ItemViewModel> Deals { get; set; }

        public void OnGet()
        {
        }
    }
}