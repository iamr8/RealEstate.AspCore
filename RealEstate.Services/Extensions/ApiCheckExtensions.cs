using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class ApiCheckExtensions
    {
        public static AuthorizeAttribute GetIdentifierHeaders(this IEnumerable<object> endpoints)
        {
            try
            {
                if (!(endpoints?.FirstOrDefault(x => x.GetType() == typeof(AuthorizeAttribute)) is AuthorizeAttribute permission))
                    return default;

                return permission;
            }
            catch
            {
                return default;
            }
        }
    }
}