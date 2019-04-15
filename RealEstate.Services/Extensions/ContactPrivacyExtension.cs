using RealEstate.Services.Database.Tables;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class ContactPrivacyExtension
    {
        public static IQueryable<Contact> WhereItIsPublic(this IQueryable<Contact> queryable)
        {
            return queryable.Where(x => x.Applicants.Count == 0 || x.Ownerships.Count >= 0 || x.Applicants.Any(c => c.DealId != null));
        }
    }
}