using System;

namespace RealEstate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchParameterAttribute : Attribute
    {
        public Type Type { get; set; }
        public string ParameterName { get; set; }

        public SearchParameterAttribute(string parameterName, Type type = null)
        {
            Type = type;
            ParameterName = parameterName;
        }
    }
}