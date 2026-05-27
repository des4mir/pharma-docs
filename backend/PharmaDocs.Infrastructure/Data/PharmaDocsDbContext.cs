using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Domain.Enums;

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
    private const string SeedPasswordHash = "$2a$11$gC76SgMbnnNGOHdy6WKR/uaAL2ZkBonnlNbdr5M/bLYCb8C1NTGBu";

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
                PasswordHash = SeedPasswordHash,
                Role = UserRole.RegAffairsOfficer,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = user2Id,
                FullName = "James Okafor",
                Email = "james@pharmadocs.ca",
                PasswordHash = SeedPasswordHash,
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

        var doc1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var doc2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        modelBuilder.Entity<DocumentRecord>().HasData(
            new DocumentRecord
            {
                Id = doc1Id,
                Title = "Atorvastatin Product Monograph v1.0",
                Type = DocumentType.ProductMonograph,
                Status = DocumentStatus.Final,
                Version = "1.0",
                Date = new DateOnly(2026, 1, 20),
                Notes = "Initial approved monograph",
                ProductId = product1Id,
                CreatedById = user1Id,
                CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new DocumentRecord
            {
                Id = doc2Id,
                Title = "Metformin Label Draft",
                Type = DocumentType.Label,
                Status = DocumentStatus.Draft,
                Version = "1.0",
                Date = new DateOnly(2026, 2, 1),
                Notes = null,
                ProductId = product2Id,
                CreatedById = user1Id,
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}