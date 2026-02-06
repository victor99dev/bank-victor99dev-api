using bank.victor99dev.Infrastructure.Messaging.Outbox.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank.victor99dev.Infrastructure.Database.Configuration;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(128);

        builder.Property(x => x.AggregateId)
            .HasMaxLength(128);

        builder.Property(x => x.Key)
            .HasMaxLength(128);

        builder.Property(x => x.ProcessedOnUtc);

        builder.Property(x => x.Error)
            .HasMaxLength(4000);

        builder.Property(x => x.Attempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.NextAttemptOnUtc);

        builder.Property(x => x.LockedBy)
            .HasMaxLength(128);

        builder.Property(x => x.LockedUntilUtc);

        builder.Property(x => x.CreatedOnUtc)
            .IsRequired();
    }
}
