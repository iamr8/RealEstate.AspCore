using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RealEstate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class R8AllowedFileTypesAttribute : RequiredAttribute, IClientModelValidator
    {
        private const string DefaultErrorMessage = "فایلهای مجاز: {0}";
        public IEnumerable<string> ValidTypes { get; set; }

        public R8AllowedFileTypesAttribute(params string[] type)
        {
            Process(type);
        }

        private void Process(params string[] type)
        {
            ValidTypes = type;
            ErrorMessage = string.Format(DefaultErrorMessage, string.Join(@" یا ", ValidTypes));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ErrorMessageString);

            var finalFiles = new List<IFormFile>();
            switch (value)
            {
                case IEnumerable<IFormFile> files:
                    finalFiles.AddRange(files);
                    break;

                case IFormFile file:
                    finalFiles.Add(file);
                    break;

                default:
                    return new ValidationResult(ErrorMessageString);
            }

            return finalFiles.Any(uploadedFile => uploadedFile != null && !ValidTypes.Any(e => uploadedFile.FileName.EndsWith(e)))
                ? new ValidationResult(ErrorMessageString)
                : ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-filetype",
                string.Format(DefaultErrorMessage, string.Join(", ", ValidTypes)));
            MergeAttribute(context.Attributes, "data-val-validtypes", string.Join(",", ValidTypes));
        }

        private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
                return;

            attributes.Add(key, value);
        }
    }
}