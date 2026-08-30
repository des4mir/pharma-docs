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
    }
}
