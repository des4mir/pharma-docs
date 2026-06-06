using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Application.DTOs;

public class SubmissionPackageResponseDto
{
    public Guid Id { get; set; }
    public SubmissionType SubmissionType { get; set; }
    public string RegulatoryBody { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; }
    public DateOnly? TargetDate { get; set; }
    public DateOnly? SubmissionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
}