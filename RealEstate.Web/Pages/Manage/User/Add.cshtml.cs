using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.User
{
    [NavBarHelper(typeof(IndexModel))]
    public class AddModel : AddPageModel
    {
        private readonly IUserService _userService;
        private readonly IBaseService _baseService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IUserService userService,
            IBaseService baseService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _userService = userService;
            _localizer = sharedLocalizer;
            _baseService = baseService;
        }

        [BindProperty]
        public UserInputViewModel NewUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            var result = await this.OnGetHandlerAsync(id, status,
                identifier => _userService.FindInputAsync(identifier),
                typeof(IndexModel).Page(),
                true,
                 () =>
                {
                    if (!User.IsInRole(nameof(Role.User)))
                        return null;

                    var currentUser = _baseService.CurrentUser();
                    return currentUser != null ? _userService.FindInputAsync(currentUser.Id) : null;
                });
            return result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await this.OnPostHandlerAsync(
                () => _userService.AddOrUpdateAsync(NewUser, !NewUser.IsNew, true),
                typeof(IndexModel).Page(),
                typeof(AddModel).Page());
            return result;
        }
    }
}