using System;

namespace RealEstate.Base
{
    public static class BaseIncludeExtensions
    {
        public static TModel R8Include<TModel>(this TModel source, Action<TModel> property)
        {
            if (source == null)
                return default;

            property.Invoke(source);
            return source;
        }
    }
}