using RealEstate.Base.Api;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class ApiCheckExtensions
    {
        public static UserResponse GetUserOfToken(this IEnumerable<object> endpoints)
        {
            try
            {
                if (!(endpoints?.FirstOrDefault(x => x.GetType() == typeof(AuthorizeApiAttribute)) is AuthorizeApiAttribute permission))
                    return default;

                return permission.UserResponse;
            }
            catch
            {
                return default;
            }
        }
    }
}