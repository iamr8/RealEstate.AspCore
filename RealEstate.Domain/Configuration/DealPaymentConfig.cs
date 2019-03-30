﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Database;
using RealEstate.Domain.Tables;

namespace RealEstate.Domain.Configuration
{
    public class DealPaymentConfig : BaseEntityConfiguration<DealPayment>
    {
        public override void Configure(EntityTypeBuilder<DealPayment> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Deal)
                .WithMany(x => x.DealPayments)
                .HasForeignKey(x => x.DealId)
                .IsRequired();

            builder.HasMany(x => x.Pictures)
                .WithOne(x => x.DealPayment)
                .HasForeignKey(x => x.DealPaymentId);
        }
    }
}