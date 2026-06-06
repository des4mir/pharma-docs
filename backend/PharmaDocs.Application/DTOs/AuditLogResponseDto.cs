namespace PharmaDocs.Application.DTOs;

public class AuditLogResponseDto
{
    public Guid Id { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string ChangedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; }
}