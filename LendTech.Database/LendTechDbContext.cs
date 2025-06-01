using System;
using System.Collections.Generic;
using LendTech.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LendTech.Database;

public partial class LendTechDbContext : DbContext
{
    public LendTechDbContext()
    {
    }

    public LendTechDbContext(DbContextOptions<LendTechDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CurrencyRate> CurrencyRates { get; set; }

    public virtual DbSet<InboxEvent> InboxEvents { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OutboxEvent> OutboxEvents { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionGroup> PermissionGroups { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermissionGroup> RolePermissionGroups { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=LendTech;Trusted_Connection=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => new { e.OrganizationId, e.CreatedAt }, "IX_AuditLogs_OrganizationId_CreatedAt");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Action).HasMaxLength(32);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.EntityId).HasMaxLength(64);
            entity.Property(e => e.EntityName).HasMaxLength(128);
            entity.Property(e => e.IpAddress).HasMaxLength(64);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.UserAgent).HasMaxLength(256);

            entity.HasOne(d => d.Organization).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLogs_Organizations");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AuditLogs_Users");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_Currencies_Code").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Code).HasMaxLength(8);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.DecimalPlaces).HasDefaultValue(2);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(64);
            entity.Property(e => e.Symbol).HasMaxLength(8);
        });

        modelBuilder.Entity<CurrencyRate>(entity =>
        {
            entity.HasIndex(e => new { e.EffectiveDate, e.ExpiryDate }, "IX_CurrencyRates_EffectiveDate_ExpiryDate");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.FromCurrency).WithMany(p => p.CurrencyRateFromCurrencies)
                .HasForeignKey(d => d.FromCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurrencyRates_FromCurrency");

            entity.HasOne(d => d.ToCurrency).WithMany(p => p.CurrencyRateToCurrencies)
                .HasForeignKey(d => d.ToCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CurrencyRates_ToCurrency");
        });

        modelBuilder.Entity<InboxEvent>(entity =>
        {
            entity.HasIndex(e => e.MessageId, "IX_InboxEvents_MessageId").IsUnique();

            entity.HasIndex(e => e.ProcessedAt, "IX_InboxEvents_ProcessedAt");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.EventType).HasMaxLength(64);
            entity.Property(e => e.MessageId).HasMaxLength(128);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);

            entity.HasOne(d => d.Organization).WithMany(p => p.InboxEvents)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InboxEvents_Organizations");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_Organizations_Code").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Code).HasMaxLength(32);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.DeletedBy).HasMaxLength(64);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<OutboxEvent>(entity =>
        {
            entity.HasIndex(e => e.ProcessedAt, "IX_OutboxEvents_ProcessedAt").HasFilter("([ProcessedAt] IS NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.EventType).HasMaxLength(64);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);

            entity.HasOne(d => d.Organization).WithMany(p => p.OutboxEvents)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutboxEvents_Organizations");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_Permissions_Code").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Code).HasMaxLength(64);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.DeletedBy).HasMaxLength(64);
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.PermissionGroup).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.PermissionGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permissions_PermissionGroups");
        });

        modelBuilder.Entity<PermissionGroup>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_PermissionGroups_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.DeletedBy).HasMaxLength(64);
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.OrganizationId }, "IX_Roles_Name_OrganizationId").IsUnique();

            entity.HasIndex(e => e.OrganizationId, "IX_Roles_OrganizationId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.DeletedBy).HasMaxLength(64);
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.Organization).WithMany(p => p.Roles)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_Organizations");
        });

        modelBuilder.Entity<RolePermissionGroup>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionGroupId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);

            entity.HasOne(d => d.PermissionGroup).WithMany(p => p.RolePermissionGroups)
                .HasForeignKey(d => d.PermissionGroupId)
                .HasConstraintName("FK_RolePermissionGroups_PermissionGroups");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissionGroups)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermissionGroups_Roles");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => new { e.Email, e.OrganizationId }, "IX_Users_Email_OrganizationId").IsUnique();

            entity.HasIndex(e => e.OrganizationId, "IX_Users_OrganizationId");

            entity.HasIndex(e => new { e.Username, e.OrganizationId }, "IX_Users_Username_OrganizationId").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.DeletedBy).HasMaxLength(64);
            entity.Property(e => e.Email).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(64);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(64);
            entity.Property(e => e.MobileNumber).HasMaxLength(32);
            entity.Property(e => e.ModifiedBy).HasMaxLength(64);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(64);

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Organizations");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserTokens_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AccessToken).HasMaxLength(256);
            entity.Property(e => e.AccessTokenExpiresAt).HasPrecision(0);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(64);
            entity.Property(e => e.IpAddress).HasMaxLength(64);
            entity.Property(e => e.RefreshToken).HasMaxLength(256);
            entity.Property(e => e.RefreshTokenExpiresAt).HasPrecision(0);
            entity.Property(e => e.RevokedAt).HasPrecision(0);
            entity.Property(e => e.RevokedBy).HasMaxLength(64);
            entity.Property(e => e.RevokedReason).HasMaxLength(256);
            entity.Property(e => e.UserAgent).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserTokens_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
