using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Domain.Entities;

public class SubmissionPackage
{
    public Guid Id { get; set; }
    public SubmissionType SubmissionType { get; set; }
    public string RegulatoryBody { get; set; } = "Health Canada";
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
    public DateOnly? TargetDate { get; set; }
    public DateOnly? SubmissionDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public Guid ProductId { get; set; }
    public Guid CreatedById { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public ICollection<SubmissionDocument> SubmissionDocuments { get; set; } = new List<SubmissionDocument>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}