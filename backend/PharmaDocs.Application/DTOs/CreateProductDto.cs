namespace PharmaDocs.Application.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? DIN { get; set; }
    public string? NPN { get; set; }
    public string MedicinalIngredient { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string DosageForm { get; set; } = string.Empty;
    public string RouteOfAdministration { get; set; } = string.Empty;
    public string TherapeuticCategory { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
}