using Microsoft.EntityFrameworkCore;
using OrphanManagement.Domain.Entities;
using OrphanManagement.Domain.Enums;

namespace OrphanManagement.Infrastructure.Data;

/// <summary>
/// Database seeder for initial data population
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds the database with initial data including users, orphans, events, and relationships
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        await SeedUsersAsync(context);
        await SeedOrphansAsync(context);
        await SeedEventsAsync(context);
        await SeedOrphanEventsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context)
    {
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Admin User",
                Email = "admin@orphan.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                Phone = "+1234567890",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Sarah Johnson",
                Email = "sarah.johnson@orphan.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Social@123"),
                Role = UserRole.SocialWorker,
                Phone = "+1234567891",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Michael Chen",
                Email = "michael.chen@orphan.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Social@123"),
                Role = UserRole.SocialWorker,
                Phone = "+1234567892",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Emma Williams",
                Email = "emma.williams@orphan.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Volunteer@123"),
                Role = UserRole.Volunteer,
                Phone = "+1234567893",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "David Brown",
                Email = "david.brown@orphan.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Volunteer@123"),
                Role = UserRole.Volunteer,
                Phone = "+1234567894",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Lisa Anderson",
                Email = "lisa.anderson@orphan.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Viewer@123"),
                Role = UserRole.Viewer,
                Phone = "+1234567895",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);
    }

    private static async Task SeedOrphansAsync(ApplicationDbContext context)
    {
        var orphans = new List<Orphan>
        {
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Ahmed",
                LastName = "Hassan",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(2015, 3, 15),
                NationalId = "ORF001",
                City = "Cairo",
                Address = "123 Nile Street, Dokki",
                HealthStatus = HealthStatus.Good,
                HealthNotes = "Regular checkups completed",
                EducationStatus = EducationStatus.Primary,
                SchoolName = "Cairo Primary School",
                ClassLevel = "Grade 4",
                GuardianName = "Fatima Hassan",
                GuardianPhone = "+201234567890",
                GuardianRelationship = "Aunt",
                SponsorshipStatus = SponsorshipStatus.Sponsored,
                Notes = "Very active in sports activities",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Mariam",
                LastName = "Ali",
                Gender = Gender.Female,
                DateOfBirth = new DateTime(2013, 7, 22),
                NationalId = "ORF002",
                City = "Alexandria",
                Address = "456 Mediterranean Avenue",
                HealthStatus = HealthStatus.Good,
                HealthNotes = null,
                EducationStatus = EducationStatus.Primary,
                SchoolName = "Alexandria Girls School",
                ClassLevel = "Grade 6",
                GuardianName = "Layla Ali",
                GuardianPhone = "+201234567891",
                GuardianRelationship = "Grandmother",
                SponsorshipStatus = SponsorshipStatus.Pending,
                Notes = "Excellent student, loves reading",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Omar",
                LastName = "Mohamed",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(2016, 11, 8),
                NationalId = "ORF003",
                City = "Giza",
                Address = "789 Pyramid Road",
                HealthStatus = HealthStatus.NeedsAttention,
                HealthNotes = "Requires regular medication for asthma",
                EducationStatus = EducationStatus.Primary,
                SchoolName = "Giza Community School",
                ClassLevel = "Grade 3",
                GuardianName = "Khalid Mohamed",
                GuardianPhone = "+201234567892",
                GuardianRelationship = "Uncle",
                SponsorshipStatus = SponsorshipStatus.NotSponsored,
                Notes = "Needs financial support for medical care",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Zainab",
                LastName = "Ibrahim",
                Gender = Gender.Female,
                DateOfBirth = new DateTime(2012, 2, 18),
                NationalId = "ORF004",
                City = "Cairo",
                Address = "321 Zamalek District",
                HealthStatus = HealthStatus.Good,
                HealthNotes = null,
                EducationStatus = EducationStatus.Secondary,
                SchoolName = "Cairo Secondary School",
                ClassLevel = "Grade 7",
                GuardianName = "Amira Ibrahim",
                GuardianPhone = "+201234567893",
                GuardianRelationship = "Older Sister",
                SponsorshipStatus = SponsorshipStatus.Sponsored,
                Notes = "Aspires to become a teacher",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Youssef",
                LastName = "Mahmoud",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(2014, 9, 5),
                NationalId = "ORF005",
                City = "Mansoura",
                Address = "555 Delta Street",
                HealthStatus = HealthStatus.Good,
                HealthNotes = null,
                EducationStatus = EducationStatus.Primary,
                SchoolName = "Mansoura Boys School",
                ClassLevel = "Grade 5",
                GuardianName = "Hoda Mahmoud",
                GuardianPhone = "+201234567894",
                GuardianRelationship = "Aunt",
                SponsorshipStatus = SponsorshipStatus.Pending,
                Notes = "Talented in mathematics",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Nour",
                LastName = "Karim",
                Gender = Gender.Female,
                DateOfBirth = new DateTime(2017, 5, 12),
                NationalId = "ORF006",
                City = "Alexandria",
                Address = "888 Corniche Road",
                HealthStatus = HealthStatus.Good,
                HealthNotes = null,
                EducationStatus = EducationStatus.Kindergarten,
                SchoolName = "Little Stars Kindergarten",
                ClassLevel = "KG2",
                GuardianName = "Salma Karim",
                GuardianPhone = "+201234567895",
                GuardianRelationship = "Grandmother",
                SponsorshipStatus = SponsorshipStatus.NotSponsored,
                Notes = "Very shy, needs socialization support",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Hassan",
                LastName = "Farid",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(2011, 12, 30),
                NationalId = "ORF007",
                City = "Cairo",
                Address = "999 Heliopolis",
                HealthStatus = HealthStatus.Good,
                HealthNotes = null,
                EducationStatus = EducationStatus.Secondary,
                SchoolName = "Heliopolis High School",
                ClassLevel = "Grade 8",
                GuardianName = "Nadia Farid",
                GuardianPhone = "+201234567896",
                GuardianRelationship = "Aunt",
                SponsorshipStatus = SponsorshipStatus.Sponsored,
                Notes = "Interested in computer science",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Orphan
            {
                Id = Guid.NewGuid(),
                FirstName = "Salma",
                LastName = "Mustafa",
                Gender = Gender.Female,
                DateOfBirth = new DateTime(2015, 6, 25),
                NationalId = "ORF008",
                City = "Giza",
                Address = "111 Dokki Square",
                HealthStatus = HealthStatus.NeedsAttention,
                HealthNotes = "Vision problems, wears glasses",
                EducationStatus = EducationStatus.Primary,
                SchoolName = "Dokki Primary School",
                ClassLevel = "Grade 4",
                GuardianName = "Mona Mustafa",
                GuardianPhone = "+201234567897",
                GuardianRelationship = "Older Sister",
                SponsorshipStatus = SponsorshipStatus.Pending,
                Notes = "Requires annual eye exams",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Orphans.AddRangeAsync(orphans);
    }

    private static async Task SeedEventsAsync(ApplicationDbContext context)
    {
        var events = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Annual Sports Day",
                Description = "A full day of sports activities and competitions for all orphans",
                StartDate = DateTime.UtcNow.AddDays(15),
                EndDate = DateTime.UtcNow.AddDays(15).AddHours(8),
                Location = "Cairo Sports Complex",
                EventType = EventType.Recreational,
                MaxParticipants = 50,
                Budget = 5000.00m,
                Notes = "Bring sports attire and water bottles",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Educational Workshop: Science for Kids",
                Description = "Interactive science experiments and demonstrations",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(7).AddHours(4),
                Location = "Cairo Science Center",
                EventType = EventType.Educational,
                MaxParticipants = 30,
                Budget = 3000.00m,
                Notes = "Age 8-14 recommended",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Health Checkup Camp",
                Description = "Comprehensive health screening and checkups for all orphans",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(5).AddHours(6),
                Location = "Community Health Center",
                EventType = EventType.Medical,
                MaxParticipants = 40,
                Budget = 8000.00m,
                Notes = "Bring health records if available",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Eid Celebration",
                Description = "Special Eid celebration with gifts, food, and entertainment",
                StartDate = DateTime.UtcNow.AddDays(20),
                EndDate = DateTime.UtcNow.AddDays(20).AddHours(5),
                Location = "Community Center Hall",
                EventType = EventType.Cultural,
                MaxParticipants = 100,
                Budget = 10000.00m,
                Notes = "All orphans and guardians invited",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Art & Crafts Workshop",
                Description = "Creative arts and crafts session for developing artistic skills",
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(10).AddHours(3),
                Location = "Arts Center, Alexandria",
                EventType = EventType.Educational,
                MaxParticipants = 25,
                Budget = 2000.00m,
                Notes = "Materials will be provided",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Title = "Summer Camp 2024",
                Description = "Week-long summer camp with various activities",
                StartDate = DateTime.UtcNow.AddDays(60),
                EndDate = DateTime.UtcNow.AddDays(67),
                Location = "Camp Green Valley",
                EventType = EventType.Recreational,
                MaxParticipants = 60,
                Budget = 25000.00m,
                Notes = "Overnight accommodation included",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Events.AddRangeAsync(events);
    }

    private static async Task SeedOrphanEventsAsync(ApplicationDbContext context)
    {
        var orphans = await context.Orphans.ToListAsync();
        var events = await context.Events.ToListAsync();

        if (!orphans.Any() || !events.Any())
        {
            return;
        }

        var orphanEvents = new List<OrphanEvent>();

        for (int i = 0; i < Math.Min(3, events.Count); i++)
        {
            for (int j = 0; j < Math.Min(5, orphans.Count); j++)
            {
                orphanEvents.Add(new OrphanEvent
                {
                    Id = Guid.NewGuid(),
                    OrphanId = orphans[j].Id,
                    EventId = events[i].Id,
                    AttendanceStatus = j % 3 == 0 ? AttendanceStatus.Registered : 
                                     j % 3 == 1 ? AttendanceStatus.Attended : AttendanceStatus.Absent,
                    Notes = j % 2 == 0 ? "Looking forward to this event" : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await context.OrphanEvents.AddRangeAsync(orphanEvents);
    }
}
