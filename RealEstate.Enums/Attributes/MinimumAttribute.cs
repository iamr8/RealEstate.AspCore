using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MinimumAttribute : ValidationAttribute
    {
        public double Value { get; set; }

        public MinimumAttribute(double value) : base(() => $"مقدار باید حداقل {value} باشد")
        {
            Value = value;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Value);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(ErrorMessageString);

            var val = Convert.ToDouble(value);
            if (val < Value)
                return new ValidationResult(ErrorMessageString);

            return ValidationResult.Success;
        }
    }
}