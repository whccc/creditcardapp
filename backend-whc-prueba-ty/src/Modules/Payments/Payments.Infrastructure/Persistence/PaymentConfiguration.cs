using CreditCardApp.Modules.Payments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditCardApp.Modules.Payments.Infrastructure.Persistence;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Id).ValueGeneratedNever();

        builder.Property(payment => payment.UserId).IsRequired();
        builder.Property(payment => payment.CardId).IsRequired();
        builder.HasIndex(payment => new { payment.UserId, payment.CreatedAt });
        builder.HasIndex(payment => new { payment.UserId, payment.CardId });

        builder.Property(payment => payment.CardLast4Digits).HasMaxLength(4).IsFixedLength().IsRequired();
        builder.Property(payment => payment.CardBrand).HasMaxLength(20).IsRequired();
        builder.Property(payment => payment.Currency).HasMaxLength(3).IsFixedLength().IsRequired();

        builder.Property(payment => payment.Amount)
            .HasConversion(amount => amount.Value, value => PaymentAmount.FromPersistence(value))
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.Merchant)
            .HasConversion(merchant => merchant.Name, value => Merchant.FromPersistence(value))
            .HasMaxLength(Merchant.MaxLength)
            .IsRequired();

        builder.Property(payment => payment.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(payment => payment.DeclineReason).HasMaxLength(Payment.DeclineReasonMaxLength);
        builder.Property(payment => payment.CreatedAt).IsRequired();
    }
}
