using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Enums;
using System.Linq;

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

            builder.HasQueryFilter(entity => string.IsNullOrEmpty(entity.Audit)
                                                      || entity.Audits.OrderByDescending(x => x.DateTime).FirstOrDefault().Type != LogTypeEnum.Delete);
        }
    }
}