using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrphanManagement.Domain.Entities;

namespace OrphanManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Orphan entity.
/// Defines table structure, constraints, indexes, and relationships.
/// </summary>
public class OrphanConfiguration : IEntityTypeConfiguration<Orphan>
{
    public void Configure(EntityTypeBuilder<Orphan> builder)
    {
        // Table name
        builder.ToTable("Orphans");

        // Primary key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Gender)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.DateOfBirth)
            .IsRequired();

        builder.Property(o => o.NationalId)
            .HasMaxLength(50);

        builder.Property(o => o.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Address)
            .HasMaxLength(200);

        builder.Property(o => o.HealthStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.HealthNotes)
            .HasMaxLength(500);

        builder.Property(o => o.EducationStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.SchoolName)
            .HasMaxLength(200);

        builder.Property(o => o.ClassLevel)
            .HasMaxLength(50);

        builder.Property(o => o.GuardianName)
            .HasMaxLength(100);

        builder.Property(o => o.GuardianPhone)
            .HasMaxLength(20);

        builder.Property(o => o.GuardianRelationship)
            .HasMaxLength(50);

        builder.Property(o => o.SponsorshipStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired();

        // Computed columns (not mapped to database)
        builder.Ignore(o => o.FullName);
        builder.Ignore(o => o.Age);

        // Relationships
        builder.HasMany(o => o.Sponsorships)
            .WithOne(s => s.Orphan)
            .HasForeignKey(s => s.OrphanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.OrphanEvents)
            .WithOne(oe => oe.Orphan)
            .HasForeignKey(oe => oe.OrphanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(o => o.NationalId)
            .IsUnique()
            .HasDatabaseName("IX_Orphans_NationalId")
            .HasFilter("[NationalId] IS NOT NULL"); // Partial index for SQL Server

        builder.HasIndex(o => o.City)
            .HasDatabaseName("IX_Orphans_City");

        builder.HasIndex(o => o.SponsorshipStatus)
            .HasDatabaseName("IX_Orphans_SponsorshipStatus");

        builder.HasIndex(o => o.DateOfBirth)
            .HasDatabaseName("IX_Orphans_DateOfBirth");
    }
}
