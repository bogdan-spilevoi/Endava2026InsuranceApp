using FluentValidation;
using InsuranceApp.Application.Common.Validation;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Application.Features.Clients.CreateClient;

public class CreateOrderCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.IdentificationNumber)
            .NotEmpty()
                .WithMessage("Identification number is required.")
            .MaximumLength(32)
                .WithMessage("Maximim identification number length is 32.");
       
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name cannot be empty.")
            .MaximumLength(200)
                .WithMessage("Maximim name length is 200.");

        RuleFor(x => x.Email).MustBeEmail();

        RuleFor(x => x.PhoneNumber).MustBePhoneNumber();

        RuleFor(x => x)
            .Must(cmd => ValueObjectRules.Try(() =>
                _ = new Address(cmd.Street, cmd.Number, cmd.Additional)))
            .WithMessage("Invalid address.");
            
    }
}