using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LendTech.Database.Entities;

namespace LendTech.Database.Configurations;

/// <summary>
/// پیکربندی موجودیت رویداد Inbox
/// </summary>
public class InboxEventConfiguration : IEntityTypeConfiguration<InboxEvent>
{
    public void Configure(EntityTypeBuilder<InboxEvent> builder)
    {
        builder.ToTable("InboxEvents");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(x => x.MessageId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EventData)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => x.MessageId)
            .IsUnique();

        builder.HasIndex(x => x.ProcessedAt);

        // روابط
        builder.HasOne(x => x.Organization)
            .WithMany(x => x.InboxEvents)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
