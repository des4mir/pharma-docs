using PharmaDocs.Domain.Enums;

namespace PharmaDocs.Application.DTOs;

public class UpdateSubmissionStatusDto
{
    public SubmissionStatus NewStatus { get; set; }
    public string? Notes { get; set; }
}