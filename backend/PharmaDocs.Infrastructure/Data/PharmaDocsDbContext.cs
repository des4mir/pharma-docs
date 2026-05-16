using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Domain.Enums;
using BCrypt.Net;

namespace PharmaDocs.Infrastructure.Data;

public class PharmaDocsDbContext : DbContext
{
    public PharmaDocsDbContext(DbContextOptions<PharmaDocsDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<DocumentRecord> DocumentRecords => Set<DocumentRecord>();
    public DbSet<SubmissionPackage> SubmissionPackages => Set<SubmissionPackage>();
    public DbSet<SubmissionDocument> SubmissionDocuments => Set<SubmissionDocument>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for join table
        modelBuilder.Entity<SubmissionDocument>()
            .HasKey(sd => new { sd.SubmissionPackageId, sd.DocumentRecordId });

        // Enums stored as strings
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<DocumentRecord>()
            .Property(d => d.Type)
            .HasConversion<string>();

        modelBuilder.Entity<DocumentRecord>()
            .Property(d => d.Status)
            .HasConversion<string>();

        modelBuilder.Entity<SubmissionPackage>()
            .Property(s => s.SubmissionType)
            .HasConversion<string>();

        modelBuilder.Entity<SubmissionPackage>()
            .Property(s => s.Status)
            .HasConversion<string>();

        // --- SEED DATA ---

        var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = user1Id,
                FullName = "Sarah Leblanc",
                Email = "sarah@pharmadocs.ca",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234!"),
                Role = UserRole.RegAffairsOfficer,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = user2Id,
                FullName = "James Okafor",
                Email = "james@pharmadocs.ca",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234!"),
                Role = UserRole.Viewer,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        var product1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var product2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = product1Id,
                Name = "Atorvastatin 20mg Tablet",
                DIN = "02245276",
                NPN = null,
                MedicinalIngredient = "Atorvastatin Calcium",
                Manufacturer = "Apotex Inc.",
                DosageForm = "Tablet",
                RouteOfAdministration = "Oral",
                TherapeuticCategory = "Cardiovascular",
                CreatedById = user1Id,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = product2Id,
                Name = "Metformin 500mg Tablet",
                DIN = "02162512",
                NPN = null,
                MedicinalIngredient = "Metformin Hydrochloride",
                Manufacturer = "Teva Canada",
                DosageForm = "Tablet",
                RouteOfAdministration = "Oral",
                TherapeuticCategory = "Antidiabetic",
                CreatedById = user1Id,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}