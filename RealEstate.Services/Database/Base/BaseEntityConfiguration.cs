using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            //                .ForSqlServerIsClustered();

            //            builder.ForSqlServerIsMemoryOptimized();

            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.DateTime)
                .HasDefaultValueSql("getdate()");
        }
    }
}