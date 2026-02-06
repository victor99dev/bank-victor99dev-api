using bank.victor99dev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank.victor99dev.Infrastructure.Database.Configuration;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.AccountName)
            .Property(p => p.Value)
            .HasColumnName("account_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.Cpf)
            .Property(p => p.Value)
            .HasColumnName("cpf")
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");
    }
}