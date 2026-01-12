using FluentAssertions;
using InsuranceApp.Domain.Common;

namespace InsuranceApp.UnitTests.Domain.Common;

public sealed class EntityTests
{
    private sealed class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Equals_ShouldBeTrue_ForSameIdAndSameType()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity(id);
        var b = new TestEntity(id);

        a.Equals(b).Should().BeTrue();
        b.Equals(a).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldBeFalse_ForDifferentId()
    {
        var a = new TestEntity(Guid.NewGuid());
        var b = new TestEntity(Guid.NewGuid());

        a.Equals(b).Should().BeFalse();
        a.GetHashCode().Should().NotBe(b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenOtherIsNull()
    {
        var a = new TestEntity(Guid.NewGuid());

        a.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldBeFalse_WhenOtherIsDifferentType()
    {
        var a = new TestEntity(Guid.NewGuid());

        a.Equals("not an entity").Should().BeFalse();
    }
}
