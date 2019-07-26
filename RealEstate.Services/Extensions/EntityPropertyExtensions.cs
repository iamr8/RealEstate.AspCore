using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using GeoAPI.Geometries;
using RealEstate.Base;
using RealEstate.Services.Database.Base;

namespace RealEstate.Services.Extensions
{
    public static class EntityPropertyExtensions
    {
        public static Dictionary<string, object> GetProperties<TSource>(this TSource entity) where TSource : BaseEntity
        {
            return entity
                .GetPublicProperties()
                .Where(x => (x.PropertyType == typeof(string)
                             || x.PropertyType == typeof(int)
                             || x.PropertyType == typeof(decimal)
                             || x.PropertyType == typeof(double)
                             || x.PropertyType == typeof(IPoint)
                             || x.PropertyType == typeof(DateTime)
                             || x.PropertyType == typeof(Enum))
                            && x.Name != nameof(entity.Id)
                            && x.Name != nameof(entity.Audit)
                            && x.GetCustomAttribute<NotMappedAttribute>() == null
                            && !x.GetGetMethod().IsVirtual)
                .ToDictionary(x => x.Name, x => x.GetValue(entity));
        }
    }
}