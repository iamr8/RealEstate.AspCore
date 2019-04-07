using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services;
using RealEstate.ViewModels.Input;
using System.Threading.Tasks;

namespace RealEstate.Web.Pages.Manage.Applicant
{
    public class AddModel : PageModel
    {
        private readonly IContactService _contactService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AddModel(
            IContactService contactService,
            IStringLocalizer<SharedResource> sharedLocalizer
            )
        {
            _contactService = contactService;
            _localizer = sharedLocalizer;
        }

        [BindProperty]
        public ApplicantInputViewModel NewApplicant { get; set; }

        [ViewData]
        public string PageTitle { get; set; }

        [TempData]
        public string ApplicantStatus { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!User.IsInRole(nameof(Role.SuperAdmin)) && !User.IsInRole(nameof(Role.Admin)))
                    return Forbid();

                var model = await _contactService.ApplicantInputAsync(id).ConfigureAwait(false);
                if (model == null)
                    return RedirectToPage(typeof(Applicant.IndexModel).Page());

                NewApplicant = model;
                PageTitle = _localizer["EditApplicant"];
            }
            else
            {
                PageTitle = _localizer["NewApplicant"];
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //            var finalStatus = ModelState.IsValid
            //                ? (await _contactService.CategoryAddOrUpdateAsync(NewApplicant, !NewApplicant.IsNew, true).ConfigureAwait(false)).Item1
            //                : StatusEnum.RetryAfterReview;

            //            ApplicantStatus = finalStatus.GetDisplayName();
            //            if (finalStatus != StatusEnum.Success || !NewApplicant.IsNew)
            //                return Page();

            ModelState.Clear();
            NewApplicant = default;

            var routeAnonymous = new
            {
                id = NewApplicant?.Id
            };
            var routeValues = HtmlHelper.AnonymousObjectToHtmlAttributes(routeAnonymous);
            return RedirectToPage(typeof(Category.AddModel).Page(), routeValues);
        }
    }
}