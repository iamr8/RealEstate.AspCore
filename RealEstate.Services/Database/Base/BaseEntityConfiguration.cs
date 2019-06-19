using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Enums;
using System.Linq;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.Database.Base
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var name = typeof(TEntity).Name;
            builder.ToTable(name);

            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.HasQueryFilter(entity => Convert.ToInt32(QueryFilterExtensions.JsonValue(entity.Audit, "$[0].t")) != (int)LogTypeEnum.Delete);
        }
    }
}