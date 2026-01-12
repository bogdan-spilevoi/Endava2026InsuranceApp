using FluentAssertions;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Metadata;

namespace InsuranceApp.UnitTests.Domain.Metadata;

 public sealed class CurrencyTests
{
    [Fact]
    public void Ctor_ShouldNormalizeCode_AndSetActive()
    {
        var c = new Currency(TestIds.New(), "ron", "Romanian Leu", true);
        c.Code.Should().Be("RON");
        c.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenCodeOrNameInvalid()
    {
        var act1 = () => new Currency(TestIds.New(), "", "Name", true);
        act1.Should().Throw<DomainException>();

        var act2 = () => new Currency(TestIds.New(), "RON", "", true);
        act2.Should().Throw<DomainException>();
    }

    [Fact]
    public void ActivateDeactivate_ShouldToggle()
    {
        var c = DomainFactory.Currency(active: false);
        c.IsActive.Should().BeFalse();

        c.Activate();
        c.IsActive.Should().BeTrue();

        c.Deactivate();
        c.IsActive.Should().BeFalse();
    }
}