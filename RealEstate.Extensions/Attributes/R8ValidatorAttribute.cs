using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RealEstate.Base;

namespace RealEstate.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class R8ValidatorAttribute : ValidationAttribute, IClientModelValidator
    {
        public string Pattern { get; set; }
        public string Caution { get; set; }

        public R8ValidatorAttribute(RegexPatterns pattern)
        {
            Pattern = pattern.GetDisplayName();
            Caution = pattern.GetDescription();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            return !Regex.Match(value.ToString(), Pattern).Success
                ? new ValidationResult(Caution)
                : ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-regex", Caution);
            MergeAttribute(context.Attributes, "data-val-regex-pattern", Pattern);
        }

        private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key)) return;

            attributes.Add(key, value);
        }
    }
}