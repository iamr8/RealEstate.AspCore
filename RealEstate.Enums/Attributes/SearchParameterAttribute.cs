using System;

namespace RealEstate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchParameterAttribute : Attribute
    {
        public string ParameterName { get; set; }

        public SearchParameterAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
}