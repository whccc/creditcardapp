using CreditCardApp.Modules.Authentication.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditCardApp.Modules.Authentication.Infrastructure.Persistence;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();

        builder.Property(user => user.Email)
            .HasConversion(email => email.Value, value => Email.FromPersistence(value))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();
        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.FullName).HasMaxLength(User.FullNameMaxLength).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();
    }
}
