using Microsoft.EntityFrameworkCore;
using PharmaDocs.Domain.Entities;

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

        // SubmissionDocument primary key is a composite key
        modelBuilder.Entity<SubmissionDocument>()
            .HasKey(sd => new { sd.SubmissionPackageId, sd.DocumentRecordId });

        // Store enums as their string name, not as numbers, making the database more readable 
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
    }
}