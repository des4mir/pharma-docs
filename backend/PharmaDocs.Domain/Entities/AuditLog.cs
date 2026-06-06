namespace PharmaDocs.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }

    public string EntityType { get; set; } = string.Empty;   // "Product" | "DocumentRecord" | "SubmissionPackage"
    public Guid EntityId { get; set; }

    // What happened
    public string Action { get; set; } = string.Empty;       // "Created" | "Updated" | "StatusChanged" | "Archived"
    public string? OldValues { get; set; }                   // JSON snapshot — null for Created actions
    public string? NewValues { get; set; }                   // JSON snapshot — null for Archived-only actions

    // Keep these for backwards compat with existing PATCH /status logic
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;

    public string ChangedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public Guid? SubmissionPackageId { get; set; }           // nullable — only set for submission status changes
    public Guid ChangedById { get; set; }

    // Navigation properties
    public SubmissionPackage? SubmissionPackage { get; set; }
    public User ChangedBy { get; set; } = null!;
}