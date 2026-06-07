namespace PharmaDocs.Application.DTOs;

public class AuditLogResponseDto
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public Guid ChangedById { get; set; }
    public string ChangedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; }
}