using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LendTech.Database.Entities;

namespace LendTech.Database.Configurations;

/// <summary>
/// پیکربندی موجودیت رویداد Outbox
/// </summary>
public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("OutboxEvents");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(x => x.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EventData)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // ایندکس‌ها
        builder.HasIndex(x => x.ProcessedAt)
            .HasFilter("ProcessedAt IS NULL");

        // روابط
        builder.HasOne(x => x.Organization)
            .WithMany(x => x.OutboxEvents)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
