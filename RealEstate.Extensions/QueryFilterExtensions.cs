using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Extensions
{
    public static class QueryFilterExtensions
    {
        public static IQueryable<TEntity> Filtered<TEntity>(this IQueryable<TEntity> entities) where TEntity : BaseEntity
        {
            var result = entities.Where(entity => entity.Logs.Count == 0
                                                  || entity.Logs.LastLog().Type != TrackTypeEnum.Delete);
            return result;
        }

        public static Log LastLog(this ICollection<Log> log)
        {
            var result = log.OrderByDescending(x => x.DateTime).FirstOrDefault();
            return result;
        }

        public static Log LastLog<TEntity>(this TEntity entity) where TEntity : BaseEntity
        {
            var result = entity.Logs.LastLog();
            return result;
        }
    }
}