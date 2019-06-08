using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class DbContextExtensions
    {
        public static IQueryable<object> AsQueryable(this DbContext context, string entityName)
        {
            var model = context.Model;

            var nameSpace = context.GetType().Namespace;
            var entityFullName = $"{nameSpace}.{nameof(RealEstate.Services.Database.Tables)}.{entityName}";
            var entityType = model.FindEntityType(entityFullName);
            var clrType = entityType.ClrType;
            var query = context.AsQueryable(clrType);
            return query;
        }

        public static IQueryable<object> AsQueryable(this DbContext context, Type entityType)
        {
            var contextType = typeof(DbContext);
            var setter = contextType.GetMethod(nameof(DbContext.Set));
            var iqueryable = setter.MakeGenericMethod(entityType).Invoke(context, null) as IQueryable<object>;
            return iqueryable;
        }
    }
}