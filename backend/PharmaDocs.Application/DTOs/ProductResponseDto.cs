namespace PharmaDocs.Application.DTOs;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DIN { get; set; }
    public string? NPN { get; set; }
    public string MedicinalIngredient { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string DosageForm { get; set; } = string.Empty;
    public string RouteOfAdministration { get; set; } = string.Empty;
    public string TherapeuticCategory { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}