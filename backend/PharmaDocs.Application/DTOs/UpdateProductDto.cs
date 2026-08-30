namespace PharmaDocs.Application.DTOs;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    // DIN and NPN are intentionally omitted: they represent a product's Health Canada
    // regulatory identity and are immutable after creation. To change an identifier,
    // archive this product and create a new one.
    public string MedicinalIngredient { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string DosageForm { get; set; } = string.Empty;
    public string RouteOfAdministration { get; set; } = string.Empty;
    public string TherapeuticCategory { get; set; } = string.Empty;
}