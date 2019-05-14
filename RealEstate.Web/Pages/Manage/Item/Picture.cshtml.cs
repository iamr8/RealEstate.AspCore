using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Services.ServiceLayer;
using RealEstate.Services.ViewModels.ModelBind;

namespace RealEstate.Web.Pages.Manage.Item
{
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
        public PictureInputViewModel NewPicture { get; set; }

        public List<PictureViewModel> Pictures { get; set; }

        [ViewData]
        public string PageTitle => _localizer[SharedResource.UploadPhotos];

        public string Status { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string status)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage(typeof(IndexModel).Page());

            var isValid = await _propertyService.PropertyValidate(id).ConfigureAwait(false);
            if (!isValid)
                return RedirectToPage(typeof(IndexModel).Page());

            Pictures = await _pictureService.PropertyPicturesAsync(id).ConfigureAwait(false);
            NewPicture = new PictureInputViewModel
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
            if (!ModelState.IsValid)
            {
                var invalids = ModelState.Values.Where(x => x.ValidationState == ModelValidationState.Invalid).ToList();
                var errors = new List<string>();
                foreach (var entry in invalids)
                {
                    var thisError = string.Join(" | ", entry.Errors.Select(x => x.ErrorMessage));
                    errors.Add(thisError);
                }

                var message = string.Join("<br/>", errors);
                return RedirectToPage(typeof(PictureModel).Page(), new
                {
                    id = NewPicture?.PropertyId,
                    status = message
                });
            }

            var status = await _pictureService.PropertyPictureAddAsync(NewPicture, true).ConfigureAwait(false);
            return RedirectToPage(typeof(PictureModel).Page(), new
            {
                id = NewPicture?.PropertyId,
                status = status.GetDisplayName()
            });
        }
    }
}