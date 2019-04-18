using RealEstate.Services.Database.Tables;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class CustomerPrivacyExtension
    {
        public static IQueryable<Customer> WhereItIsPublic(this IQueryable<Customer> queryable)
        {
            return queryable.Where(x => x.Applicants.Count == 0 || x.Ownerships.Count >= 0 || x.Applicants.Any(c => c.DealId != null));
        }
    }
}