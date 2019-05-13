using Microsoft.AspNetCore.Mvc.ModelBinding;
using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.Extensions
{
    public static class ModelStateExtensions
    {
        public static (bool, List<string>) IsValid(this ModelStateDictionary modelState, string bindedProperty = null)
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));

            var ms = string.IsNullOrEmpty(bindedProperty)
                ? modelState.ToList()
                : modelState.Where(x => x.Key.StartsWith($"{bindedProperty}.")).ToList();

            var isModelStateValid = ms.All(x => x.Value.ValidationState == ModelValidationState.Valid);
            if (isModelStateValid)
                return new ValueTuple<bool, List<string>>(true, null);

            var invalids = ms.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).ToList();
            var errors = new List<string>();
            foreach (var entry in invalids)
            {
                var thisError = string.Join(" | ", entry.Value.Errors.Select(x => x.ErrorMessage));
                errors.Add(thisError);
            }

            return new ValueTuple<bool, List<string>>(false, errors);
        }

        public static async Task<ModelStateValidation> IsValidAsync(this ModelStateDictionary modelState, Func<Task<StatusEnum>> action, string bindedProperty = null)
        {
            var (isValid, errors) = modelState.IsValid(bindedProperty);
            string message;
            StatusEnum state;
            if (isValid)
            {
                var method = await action().ConfigureAwait(false);
                message = method.GetDisplayName();
                state = method;
            }
            else
            {
                message = string.Join("<br>", errors);
                state = StatusEnum.RetryAfterReview;
            }
            return new ModelStateValidation(state, message);
        }

        public static Task<ModelStateValidation> IsValidAsync<T>(this ModelStateDictionary modelState, Func<Task<MethodStatus<T>>> action, string bindedProperty = null) where T : class
        {
            return modelState.IsValidAsync(async () => (await action().ConfigureAwait(false)).Status, bindedProperty);
        }
    }
}