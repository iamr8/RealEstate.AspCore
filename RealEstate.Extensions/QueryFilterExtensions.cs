using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using System.Linq;
using RealEstate.Base.Database;

namespace RealEstate.Extensions
{
    public static class QueryFilterExtensions
    {
        public static IQueryable<TEntity> Filtered<TEntity>(this IQueryable<TEntity> mainTable, IQueryable<Log> logTable) where TEntity : BaseEntity
        {
            return from entity in mainTable
                   join log in logTable on entity.Id equals log.EntityId into logs
                   group new
                   {
                       Entity = entity,
                       Logs = logs.DefaultIfEmpty()
                   } by entity.Id
                   into loggedEntities
                   from loggedEntity in loggedEntities
                   let lastLog = loggedEntity.Logs.OrderByDescending(x => x.DateTime).FirstOrDefault()
                   where !loggedEntity.Logs.Any() || lastLog.Type != TrackTypeEnum.Delete
                   select loggedEntity.Entity;
        }

        public static Log CurrentState(this IQueryable<Log> logTable, string entityId)
        {
            return logTable.Where(x => x.EntityId == entityId).OrderByDescending(x => x.DateTime).FirstOrDefault();
        }
    }
}