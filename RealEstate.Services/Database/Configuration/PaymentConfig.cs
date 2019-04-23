using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.Database.Configuration
{
    public class PaymentConfig : BaseEntityConfiguration<Payment>
    {
        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            base.Configure(builder);
            builder.HasOne(x => x.Employee)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.EmployeeId)
                .IsRequired();

            builder.HasOne(x => x.Checkout)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.CheckoutId);
        }
    }
}