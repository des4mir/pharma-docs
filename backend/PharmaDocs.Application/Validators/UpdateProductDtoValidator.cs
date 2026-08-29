using FluentValidation;
using PharmaDocs.Application.DTOs;

namespace PharmaDocs.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(p => p.MedicinalIngredient)
            .NotEmpty().WithMessage("MedicinalIngredient is required.")
            .MaximumLength(200);

        RuleFor(p => p.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer is required.")
            .MaximumLength(200);

        RuleFor(p => p.DosageForm)
            .NotEmpty().WithMessage("DosageForm is required.")
            .MaximumLength(100);

        RuleFor(p => p.RouteOfAdministration)
            .NotEmpty().WithMessage("RouteOfAdministration is required.")
            .MaximumLength(100);

        RuleFor(p => p.TherapeuticCategory)
            .NotEmpty().WithMessage("TherapeuticCategory is required.")
            .MaximumLength(100);

        // A product is either a Health Canada–authorized drug (DIN) or a natural
        // health product (NPN) — never both, and never neither.
        RuleFor(p => p)
            .Must(HaveExactlyOneIdentifier)
            .WithMessage("A product must have exactly one of DIN or NPN, not both and not neither.")
            .WithName("DIN");

        RuleFor(p => p.DIN)
            .Matches(@"^\d{8}$").WithMessage("DIN must be exactly 8 digits.")
            .When(p => !string.IsNullOrWhiteSpace(p.DIN));

        RuleFor(p => p.NPN)
            .Matches(@"^\d{8}$").WithMessage("NPN must be exactly 8 digits.")
            .When(p => !string.IsNullOrWhiteSpace(p.NPN));
    }

    private static bool HaveExactlyOneIdentifier(UpdateProductDto dto)
    {
        var hasDin = !string.IsNullOrWhiteSpace(dto.DIN);
        var hasNpn = !string.IsNullOrWhiteSpace(dto.NPN);
        return hasDin ^ hasNpn;
    }
}
