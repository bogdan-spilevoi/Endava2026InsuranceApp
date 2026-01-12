using InsuranceApp.Domain.Common;

namespace InsuranceApp.Domain.ValueObjects;

public readonly record struct DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (end < start) throw new DomainException("End date must be >= start date.");
        Start = start;
        End = end;
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;
    public override string ToString() => $"{Start:yyyy-MM-dd} .. {End:yyyy-MM-dd}";
}