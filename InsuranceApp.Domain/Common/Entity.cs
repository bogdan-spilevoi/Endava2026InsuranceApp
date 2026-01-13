namespace InsuranceApp.Domain.Common;

public abstract class Entity<TId>(TId id)
        where TId : notnull
{
    public TId Id { get; } = id;
    public override bool Equals(object? obj)
        => obj is Entity<TId> other && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode() => Id.GetHashCode();
}