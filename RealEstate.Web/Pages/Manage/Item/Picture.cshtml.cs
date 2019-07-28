using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.ModelBind;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Item
{
    [NavBarHelper(typeof(IndexModel))]
    public class PictureModel : PageModel
    {
        private readonly IPictureService _pictureService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IPropertyService _propertyService;

        public PictureModel(
            IPictureService pictureService,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IPropertyService propertyService
            )
        {
            _pictureService = pictureService;
            _localizer = sharedLocalizer;
            _propertyService = propertyService;
        }

        [BindProperty]
        public PropertyPictureInputViewModel NewPropertyPicture { get; set; }

        public List<PictureViewModel> Pictures { get; set; }

        [ViewData]
        public string PageTitle => _localizer[SharedResource.UploadPhotos];

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage(typeof(IndexModel).Page());

            var isValid = await _propertyService.PropertyValidate(id);
            if (!isValid)
                return RedirectToPage(typeof(IndexModel).Page());

            Pictures = await _pictureService.PropertyPicturesAsync(id);
            NewPropertyPicture = new PropertyPictureInputViewModel
            {
                PropertyId = id
            };
            Status = !string.IsNullOrEmpty(status)
                ? status
                : null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (status, message) = await ModelState.IsValidAsync(
                () => _pictureService.PictureAddAsync(NewPropertyPicture.Pictures, null, NewPropertyPicture.PropertyId, null, null, null, null, true)
                    .ConfigureAwait(false)
            );

            return RedirectToPage(typeof(PictureModel).Page(), new
            {
                id = NewPropertyPicture?.PropertyId,
                status = message
            });
        }
    }
}