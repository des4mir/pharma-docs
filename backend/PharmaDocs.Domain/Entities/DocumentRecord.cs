using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Domain.Entities;

public class DocumentRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
    public string Version { get; set; } = "1.0";
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public Guid ProductId { get; set; }
    public Guid CreatedById { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public ICollection<SubmissionDocument> SubmissionDocuments { get; set; } = new List<SubmissionDocument>();
}