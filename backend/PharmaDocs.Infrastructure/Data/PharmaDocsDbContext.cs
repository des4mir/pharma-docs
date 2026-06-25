using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;
using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Infrastructure.Data;

public class PharmaDocsDbContext : DbContext
{
    public PharmaDocsDbContext(DbContextOptions<PharmaDocsDbContext> options)
        : base(options) { }

    // DbSets using property expressions instead of direct Set<T> declarations to enable lazy initialization
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<DocumentRecord> DocumentRecords => Set<DocumentRecord>();
    public DbSet<SubmissionPackage> SubmissionPackages => Set<SubmissionPackage>();
    public DbSet<SubmissionDocument> SubmissionDocuments => Set<SubmissionDocument>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Bcrypt hash for seed users; password is "Demo1234!" hashed with bcrypt (11 rounds)
    private const string SeedPasswordHash = "$2a$11$gC76SgMbnnNGOHdy6WKR/uaAL2ZkBonnlNbdr5M/bLYCb8C1NTGBu";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SubmissionDocument is a join table; uses composite key of both foreign keys
        modelBuilder.Entity<SubmissionDocument>()
            .HasKey(sd => new { sd.SubmissionPackageId, sd.DocumentRecordId });

        // Store enum values as strings in database for readability and easier migrations
        // This allows viewing raw database values without needing type conversion
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

        // Foreign key relationships with Restrict delete behavior to prevent accidental deletion of referenced records
        // CreatedBy/ArchivedBy references use WithMany() because one user can create/archive many records
        // Product
        modelBuilder.Entity<Product>()
            .HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ArchivedBy)
            .WithMany()
            .HasForeignKey(p => p.ArchivedById)
            .OnDelete(DeleteBehavior.Restrict);

        // DocumentRecord
        modelBuilder.Entity<DocumentRecord>()
            .HasOne(d => d.CreatedBy)
            .WithMany()
            .HasForeignKey(d => d.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DocumentRecord>()
            .HasOne(d => d.ArchivedBy)
            .WithMany()
            .HasForeignKey(d => d.ArchivedById)
            .OnDelete(DeleteBehavior.Restrict);

        // SubmissionPackage
        modelBuilder.Entity<SubmissionPackage>()
            .HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SubmissionPackage>()
            .HasOne(s => s.ArchivedBy)
            .WithMany()
            .HasForeignKey(s => s.ArchivedById)
            .OnDelete(DeleteBehavior.Restrict);

        // AuditLog
        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.ChangedBy)
            .WithMany()
            .HasForeignKey(a => a.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.SubmissionPackage)
            .WithMany(s => s.AuditLogs)
            .HasForeignKey(a => a.SubmissionPackageId)
            .IsRequired(false)
            // SetNull allows audit logs to persist even if submission package is deleted, maintaining audit trail history
            .OnDelete(DeleteBehavior.SetNull);

        // --- SEED DATA ---
        // Fixed GUIDs used for seed data to ensure consistent IDs across database resets
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
                Title = "Metformin Certificate of Analysis v1.0",
                Type = DocumentType.CertificateOfAnalysis,
                Status = DocumentStatus.Draft,
                Version = "1.0",
                Date = new DateOnly(2026, 2, 1),
                Notes = null,
                ProductId = product2Id,
                CreatedById = user1Id,
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        var pkg1Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

        modelBuilder.Entity<SubmissionPackage>().HasData(
            new SubmissionPackage
            {
                Id = pkg1Id,
                SubmissionType = SubmissionType.NDS,
                RegulatoryBody = "Health Canada",
                Status = SubmissionStatus.Draft,
                ProductId = product1Id,
                CreatedById = user1Id,
                TargetDate = new DateOnly(2026, 6, 30),
                CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}