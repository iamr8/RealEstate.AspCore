using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RealEstate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public string RegularExpression { get; set; }
        public RegexPatterns Pattern { get; set; }

        public ValueValidationAttribute(RegexPatterns pattern) : base(() => pattern.GetDescription())
        {
            RegularExpression = pattern.GetDisplayName();
            this.Pattern = pattern;
            this.ErrorMessage = ErrorMessageString;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            ValidationResult result;
            if (!Regex.Match(value.ToString(), RegularExpression).Success)
                result = new ValidationResult(ErrorMessageString);
            else
                result = ValidationResult.Success;

            return result;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-regex", ErrorMessageString);
            MergeAttribute(context.Attributes, "data-val-regex-pattern", RegularExpression);
        }

        private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key)) return;

            attributes.Add(key, value);
        }
    }
}