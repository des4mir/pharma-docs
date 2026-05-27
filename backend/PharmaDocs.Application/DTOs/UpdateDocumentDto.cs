using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Application.DTOs;

public class UpdateDocumentDto
{
    public string Title { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentStatus Status { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
}