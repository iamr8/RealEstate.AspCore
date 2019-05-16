using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RealEstate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileTypeValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public IEnumerable<string> ValidTypes { get; set; }
        private string Expectation => string.Join(" یا ", ValidTypes);

        public FileTypeValidationAttribute(params string[] type) : base(() => $"فایلهای مجاز: {string.Join(" یا ", type)}")
        {
            ValidTypes = type;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Expectation);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

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

            return finalFiles.Any(uploadedFile =>
                uploadedFile != null && !ValidTypes.Any(e => Path.GetExtension(uploadedFile.FileName).Substring(1).ToLower().Equals(e.ToLower())))
                ? new ValidationResult(ErrorMessageString)
                : ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-filetype", ErrorMessageString);
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