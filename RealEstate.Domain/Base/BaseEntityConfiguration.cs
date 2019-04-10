using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Base
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            var name = typeof(TEntity).Name;
            builder.ToTable(name);

            builder.HasKey(x => x.Id);
            //                .ForSqlServerIsClustered();

            //            builder.ForSqlServerIsMemoryOptimized();

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.DateTime)
                .HasDefaultValueSql("getdate()");

            builder.Property(x => x.Logs)
                .HasConversion(value => JsonConvert.SerializeObject(value),
                    value => JsonConvert.DeserializeObject<List<LogJsonEntity>>(value));
        }
    }
}