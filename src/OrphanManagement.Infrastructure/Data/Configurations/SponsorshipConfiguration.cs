using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrphanManagement.Domain.Entities;

namespace OrphanManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Sponsorship entity.
/// Defines table structure, constraints, and relationships.
/// </summary>
public class SponsorshipConfiguration : IEntityTypeConfiguration<Sponsorship>
{
    public void Configure(EntityTypeBuilder<Sponsorship> builder)
    {
        // Table name
        builder.ToTable("Sponsorships");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.SponsorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.SponsorEmail)
            .HasMaxLength(100);

        builder.Property(s => s.SponsorPhone)
            .HasMaxLength(20);

        builder.Property(s => s.Amount)
            .IsRequired()
            .HasPrecision(18, 2); // Decimal precision for currency

        builder.Property(s => s.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD"); // ISO 4217 currency code

        builder.Property(s => s.Frequency)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Computed properties (not mapped)
        builder.Ignore(s => s.IsCurrentlyActive);

        // Relationships (defined in OrphanConfiguration)

        // Indexes
        builder.HasIndex(s => s.OrphanId)
            .HasDatabaseName("IX_Sponsorships_OrphanId");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IX_Sponsorships_IsActive");

        builder.HasIndex(s => new { s.StartDate, s.EndDate })
            .HasDatabaseName("IX_Sponsorships_DateRange");
    }
}
