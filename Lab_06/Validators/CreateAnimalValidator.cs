using FluentValidation;
using Lab_06.DTOs;

namespace Lab_06.Validators;

public class CreateAnimalValidator : AbstractValidator<CreateAnimalRequest>
{
    public CreateAnimalValidator()
    {
        RuleFor(e => e.Name).MaximumLength(200).NotEmpty().NotNull();
        RuleFor(e => e.Description).MaximumLength(200).NotEmpty();
        RuleFor(e => e.Category).MaximumLength(200).NotEmpty().NotNull();
        RuleFor(e => e.Area).MaximumLength(200).NotEmpty().NotNull();
    }
}