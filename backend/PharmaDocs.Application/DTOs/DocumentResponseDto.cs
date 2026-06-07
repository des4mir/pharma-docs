namespace PharmaDocs.Application.DTOs;

public class DocumentResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ProductId { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}