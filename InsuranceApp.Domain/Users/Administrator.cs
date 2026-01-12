using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.ValueObjects;

namespace InsuranceApp.Domain.Users;

public sealed class Administrator : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public AdminRole Role { get; private set; }

    public Administrator(Guid id, string name, Email email, AdminRole role) : base(id)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 150);
        Name = name.Trim();
        Email = email;
        Role = role;
    }

    public void Update(string name, Email email, AdminRole role)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name), 150);
        Name = name.Trim();
        Email = email;
        Role = role;
    }
}