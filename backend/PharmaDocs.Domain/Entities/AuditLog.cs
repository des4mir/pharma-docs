namespace PharmaDocs.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string ChangedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public Guid SubmissionPackageId { get; set; }
    public Guid ChangedById { get; set; }

    // Navigation properties
    public SubmissionPackage SubmissionPackage { get; set; } = null!;
    public User ChangedBy { get; set; } = null!;
}