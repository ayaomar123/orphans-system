using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrphanManagement.Domain.Entities;

namespace OrphanManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Event entity.
/// Defines table structure, constraints, indexes, and relationships.
/// </summary>
public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        // Table name
        builder.ToTable("Events");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.MaxParticipants);

        builder.Property(e => e.Budget)
            .HasPrecision(18, 2); // Decimal precision for currency

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // Computed properties (not mapped)
        builder.Ignore(e => e.IsUpcoming);
        builder.Ignore(e => e.IsOngoing);
        builder.Ignore(e => e.IsCompleted);
        builder.Ignore(e => e.ParticipantCount);
        builder.Ignore(e => e.IsFull);

        // Relationships
        builder.HasMany(e => e.OrphanEvents)
            .WithOne(oe => oe.Event)
            .HasForeignKey(oe => oe.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.StartDate)
            .HasDatabaseName("IX_Events_StartDate");

        builder.HasIndex(e => e.EventType)
            .HasDatabaseName("IX_Events_EventType");

        builder.HasIndex(e => e.Location)
            .HasDatabaseName("IX_Events_Location");
    }
}
