using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrphanManagement.Domain.Entities;

namespace OrphanManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the OrphanEvent junction table.
/// Defines the many-to-many relationship between Orphans and Events.
/// </summary>
public class OrphanEventConfiguration : IEntityTypeConfiguration<OrphanEvent>
{
    public void Configure(EntityTypeBuilder<OrphanEvent> builder)
    {
        // Table name
        builder.ToTable("OrphanEvents");

        // Composite primary key
        builder.HasKey(oe => new { oe.OrphanId, oe.EventId });

        // Properties
        builder.Property(oe => oe.AttendanceStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Domain.Enums.AttendanceStatus.Registered);

        builder.Property(oe => oe.Notes)
            .HasMaxLength(500);

        builder.Property(oe => oe.RegisteredAt)
            .IsRequired();

        // Relationships are defined in Orphan and Event configurations
        // This ensures proper foreign key constraints

        // Indexes
        builder.HasIndex(oe => oe.OrphanId)
            .HasDatabaseName("IX_OrphanEvents_OrphanId");

        builder.HasIndex(oe => oe.EventId)
            .HasDatabaseName("IX_OrphanEvents_EventId");

        builder.HasIndex(oe => oe.AttendanceStatus)
            .HasDatabaseName("IX_OrphanEvents_AttendanceStatus");
    }
}
