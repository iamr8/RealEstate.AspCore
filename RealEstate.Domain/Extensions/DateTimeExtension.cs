using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Extensions
{
    public static class DateTimeExtension
    {
        public static void HasDateGenerator<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, TProperty>> propertyExpression) where TEntity : BaseEntity
        {
            builder.Property(propertyExpression)
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd()
                .IsRequired();
        }
    }
}