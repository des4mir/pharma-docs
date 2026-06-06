namespace PharmaDocs.Application.DTOs;

public class UpdateSubmissionPackageDto
{
    public DateOnly? TargetDate { get; set; }
    public DateOnly? SubmissionDate { get; set; }
    public string RegulatoryBody { get; set; } = "Health Canada";
}