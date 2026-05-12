using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<SubmissionPackage> SubmissionPackages { get; set; } = new List<SubmissionPackage>();
    public ICollection<DocumentRecord> DocumentRecords { get; set; } = new List<DocumentRecord>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}