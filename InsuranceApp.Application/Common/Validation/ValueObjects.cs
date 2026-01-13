using FluentValidation;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Application.Common.Validation;

public static class ValueObjectRules
{
    public static IRuleBuilderOptions<T, string?> MustBeEmail<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.Must(value =>
            string.IsNullOrWhiteSpace(value) || Try(() => _ = new Email(value)))
          .WithMessage("Invalid email format.");

    public static IRuleBuilderOptions<T, string?> MustBePhoneNumber<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.Must(value =>
            string.IsNullOrWhiteSpace(value) || Try(() => _ = new PhoneNumber(value)))
          .WithMessage("Invalid phone number.");

    public static bool Try(Action factory)
    {
        try { factory(); return true; }
        catch (DomainException) { return false; }
    }
}
