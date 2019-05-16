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

        public ValueValidationAttribute(RegexPatterns pattern) : base(() => pattern.GetDescription())
        {
            RegularExpression = pattern.GetDisplayName();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            return !Regex.Match(value.ToString(), RegularExpression).Success
                ? new ValidationResult(ErrorMessageString)
                : ValidationResult.Success;
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