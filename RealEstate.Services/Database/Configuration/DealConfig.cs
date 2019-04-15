﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class DealConfig : BaseEntityConfiguration<Deal>
    {
        public override void Configure(EntityTypeBuilder<Deal> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => x.Beneficiaries)
                .WithOne(x => x.Deal)
                .HasForeignKey(x => x.DealId)
                .IsRequired();

            builder.HasOne(x => x.Item)
                .WithMany(x => x.Deals)
                .HasForeignKey(x => x.ItemId)
                .IsRequired();

            builder.HasMany(x => x.Applicants)
                .WithOne(x => x.Deal)
                .HasForeignKey(x => x.DealId);
        }
    }
}