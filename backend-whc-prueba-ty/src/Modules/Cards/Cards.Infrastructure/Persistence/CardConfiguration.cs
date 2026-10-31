using CreditCardApp.Modules.Cards.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditCardApp.Modules.Cards.Infrastructure.Persistence;

internal sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("cards");
        builder.HasKey(card => card.Id);
        builder.Property(card => card.Id).ValueGeneratedNever();

        builder.Property(card => card.OwnerId).IsRequired();
        builder.HasIndex(card => card.OwnerId);

        builder.Property(card => card.HolderName).HasMaxLength(Card.HolderNameMaxLength).IsRequired();
        builder.Property(card => card.Nickname).HasMaxLength(Card.NicknameMaxLength);
        builder.Property(card => card.Last4Digits).HasMaxLength(4).IsFixedLength().IsRequired();
        builder.Property(card => card.Brand).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(card => card.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(card => card.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(card => card.CreditLimit).HasPrecision(18, 2).IsRequired();
        builder.Property(card => card.AvailableCredit).HasPrecision(18, 2).IsRequired();
        builder.Property(card => card.CreatedAt).IsRequired();
        builder.Property(card => card.UpdatedAt).IsRequired();
        builder.Property(card => card.Version).IsConcurrencyToken();
        builder.Ignore(card => card.UsedCredit);

        builder.ComplexProperty(card => card.Expiry, expiry =>
        {
            expiry.Property(value => value.Month).HasColumnName("expiry_month").IsRequired();
            expiry.Property(value => value.Year).HasColumnName("expiry_year").IsRequired();
        });
    }
}
