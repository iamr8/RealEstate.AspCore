using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Services.Extensions
{
    public class IndexPageModel : PageModel
    {
        [ViewData]
        public string PageTitle { get; set; }

        public string Status { get; set; }
    }

    public class AddPageModel : IndexPageModel
    {
        [TempData]
        public string SerializedModel { get; set; }
    }

    public static class PageHandlerExtensions
    {
        private static PropertyInfo FindInputModel<TPage>(this TPage page) where TPage : PageModel
        {
            if (page == null)
                throw new ArgumentNullException($"{nameof(page)} must be filled");

            var modelProperty = page
                .GetPublicProperties()
                .FirstOrDefault(x =>
                    x.GetCustomAttribute<BindPropertyAttribute>() != null
                    && x.PropertyType.IsSubclassOf(typeof(BaseInputViewModel))
                    && x.Name.StartsWith("New", StringComparison.CurrentCulture));
            if (modelProperty == null)
                throw new ArgumentNullException($"{nameof(page)} must have model from subclass of {nameof(BaseInputViewModel)}");

            return modelProperty;
        }

        private static PropertyInfo FindSerializedModel<TPage>(this TPage page) where TPage : PageModel
        {
            if (page == null)
                throw new ArgumentNullException($"{nameof(page)} must be filled");

            var modelProperty = page
                .GetPublicProperties()
                .FirstOrDefault(x =>
                    x.GetCustomAttribute<TempDataAttribute>() != null
                    && x.PropertyType == typeof(string)
                    && x.Name.Equals("SerializedModel", StringComparison.CurrentCulture));
            if (modelProperty == null)
                throw new ArgumentNullException($"{nameof(page)} must have model from subclass of {nameof(String)})");

            return modelProperty;
        }

        private static IEnumerable<string> Namespaces(this Type pageType)
        {
            var nameSpace = pageType?.Namespace;
            if (string.IsNullOrEmpty(nameSpace))
                throw new Exception("Namespace must be not null or empty");

            var splitByDots = nameSpace.Split('.');
            return splitByDots;
        }

        private static PropertyInfo FindPageTitle<TPage>(this TPage page) where TPage : PageModel
        {
            if (page == null)
                throw new ArgumentNullException($"{nameof(page)} must be filled");

            var modelProperty = page
                .GetPublicProperties()
                .FirstOrDefault(x =>
                    x.GetCustomAttribute<ViewDataAttribute>() != null
                    && x.PropertyType == typeof(string)
                    && x.Name.Equals("PageTitle", StringComparison.CurrentCulture));
            if (modelProperty == null)
                throw new ArgumentNullException($"{nameof(page)} must have model from subclass of {nameof(String)})");

            return modelProperty;
        }

        private static PropertyInfo FindStatus<TPage>(this TPage page) where TPage : PageModel
        {
            if (page == null)
                throw new ArgumentNullException($"{nameof(page)} must be filled");

            var modelProperty = page
                .GetPublicProperties()
                .FirstOrDefault(x =>
                    x.PropertyType == typeof(string)
                    && x.Name.Equals("Status", StringComparison.CurrentCulture));
            if (modelProperty == null)
                throw new ArgumentNullException($"{nameof(page)} must have model from subclass of {nameof(String)})");

            return modelProperty;
        }

        public static async Task<IActionResult> OnPostHandlerAsync<TPage, TModel>(this TPage page,
            Func<Task<MethodStatus<TModel>>> action,
            string pageRedirectOnSuccess,
            string pageRedirectOnFailure) where TPage : PageModel where TModel : BaseEntity
        {
            var (status, message) = await page.ModelState.IsValidAsync(action.Invoke);

            var inputProp = page.FindInputModel();
            var inputModel = inputProp.GetValue(page);

            var serializedProp = page.FindSerializedModel();
            var serializedData = JsonConvert.SerializeObject(inputModel);
            serializedProp.SetValue(page, serializedData);

            var modelIdProp = inputModel.GetPublicProperties()
                .FirstOrDefault(x => x.Name.Equals("Id", StringComparison.CurrentCulture) && x.PropertyType == typeof(string));
            if (modelIdProp == null)
                throw new NullReferenceException($"{nameof(inputModel)} must have Id property");

            var modelId = modelIdProp.GetValue(inputModel);
            return page.RedirectToPage(status != StatusEnum.Success
                ? pageRedirectOnFailure
                : pageRedirectOnSuccess, new
                {
                    status = message,
                    id = status != StatusEnum.Success ? modelId : null
                });
        }

        private static IStringLocalizer<SharedResource> FindLocalizer<TPage>(this TPage page) where TPage : PageModel
        {
            var localizerField = page.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(x => x.FieldType.Name.StartsWith("IStringLocalizer`1", StringComparison.CurrentCulture));
            if (localizerField == null || !(localizerField.GetValue(page) is IStringLocalizer<SharedResource> localizer))
                throw new NullReferenceException($"{nameof(IStringLocalizer<SharedResource>)} must be inject into PageModel");

            return localizer;
        }

        public static async Task<IActionResult> OnGetHandlerAsync<TPage, TInputModel>(this TPage page,
            string identifier,
            string status,
            Func<string, Task<TInputModel>> model,
            string pageRedirectOnFailure,
            bool considerPermissions,
            Func<Task<TInputModel>> actionOnIdentifierNull = null)
            where TInputModel : BaseInputViewModel where TPage : PageModel
        {
            TInputModel tempModel = null;
            if (!string.IsNullOrEmpty(identifier))
            {
                if (considerPermissions)
                {
                    if (!page.User.IsInRole(nameof(Role.SuperAdmin)) && !page.User.IsInRole(nameof(Role.Admin)))
                        return page.Forbid();
                }

                tempModel = await model.Invoke(identifier);
            }
            else
            {
                if (actionOnIdentifierNull != null)
                    tempModel = await actionOnIdentifierNull.Invoke();
            }

            var serializedProp = page.FindSerializedModel();
            var serializedModel = (string)serializedProp.GetValue(page);

            var inputProp = page.FindInputModel();
            var deserializedModel = tempModel.Deserialize(identifier, serializedModel);
            inputProp.SetValue(page, deserializedModel);
            var inputModel = inputProp.GetValue(page) as TInputModel;

            serializedProp.SetValue(page, default);

            #region PageStatus

            var statusProp = page.FindStatus();
            var statusValue = !string.IsNullOrEmpty(status) ? status : null;
            statusProp.SetValue(page, statusValue);

            #endregion PageStatus

            #region PageTitle

            var pageTitleProp = page.FindPageTitle();
            var titleKey = $"{(inputModel == null ? "New" : "Edit")}{page.GetType().Namespaces().Last()}";

            var localizer = page.FindLocalizer();
            var title = localizer[titleKey].ToString();
            pageTitleProp.SetValue(page, title);

            #endregion PageTitle

            if (!string.IsNullOrEmpty(identifier) && inputModel == null)
                return page.RedirectToPage(pageRedirectOnFailure);

            return page.Page();
        }
    }
}