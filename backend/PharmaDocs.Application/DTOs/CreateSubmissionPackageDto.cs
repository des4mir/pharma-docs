using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Application.DTOs;

public class CreateSubmissionPackageDto
{
    public SubmissionType SubmissionType { get; set; }
    public Guid ProductId { get; set; }
    public DateOnly? TargetDate { get; set; }
    public string RegulatoryBody { get; set; } = "Health Canada";
}