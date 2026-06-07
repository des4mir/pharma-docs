using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Application.DTOs;

public class CreateDocumentDto
{
    public string Title { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
    public string Version { get; set; } = "1.0";
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public Guid ProductId { get; set; }
}