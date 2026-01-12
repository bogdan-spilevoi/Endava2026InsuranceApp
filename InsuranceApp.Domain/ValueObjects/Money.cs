using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }
    public string CurrencyCode { get; }
    public Money(decimal amount, string currencyCode)
    {
        Guard.NotNegative(amount, nameof(Amount));
        Guard.NotNullOrWhiteSpace(currencyCode, nameof(CurrencyCode), 3);
        
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public Money WithAmount(decimal amount) => new Money(amount, CurrencyCode);
    public override string ToString() => $"{Amount:0.00} {CurrencyCode}";
}