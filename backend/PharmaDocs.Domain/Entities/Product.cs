namespace PharmaDocs.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DIN { get; set; }          // Drug Identification Number
    public string? NPN { get; set; }          // Natural Product Number
    public string MedicinalIngredient { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string DosageForm { get; set; } = string.Empty;
    public string RouteOfAdministration { get; set; } = string.Empty;
    public string TherapeuticCategory { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedAt { get; set; }
    public Guid? ArchivedById { get; set; }
    public User? ArchivedBy { get; set; }

    // Foreign keys
    public Guid CreatedById { get; set; }

    // Navigation properties
    public User CreatedBy { get; set; } = null!;
    public ICollection<DocumentRecord> Documents { get; set; } = new List<DocumentRecord>();
    public ICollection<SubmissionPackage> SubmissionPackages { get; set; } = new List<SubmissionPackage>();
}