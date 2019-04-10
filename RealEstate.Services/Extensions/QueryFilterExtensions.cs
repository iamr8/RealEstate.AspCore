using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class QueryFilterExtensions
    {
        public static IQueryable<TEntity> Filtered<TEntity>(this IQueryable<TEntity> entities) where TEntity : BaseEntity
        {
            var result = entities.Where(entity => string.IsNullOrEmpty(entity.Audit)
                                                  || entity.Audits.OrderByDescending(x => x.DateTime).FirstOrDefault().Type != LogTypeEnum.Delete);
            return result;
        }

        public static LogJsonEntity LastLog<TEntity>(this TEntity entity) where TEntity : BaseEntity
        {
            var result = entity.Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault();
            return result;
        }
    }
}