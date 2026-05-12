namespace PharmaDocs.Domain.Entities;

public class SubmissionDocument
{
    public Guid SubmissionPackageId { get; set; }
    public Guid DocumentRecordId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public SubmissionPackage SubmissionPackage { get; set; } = null!;
    public DocumentRecord DocumentRecord { get; set; } = null!;
}