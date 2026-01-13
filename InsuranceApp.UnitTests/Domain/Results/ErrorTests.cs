using FluentAssertions;
using InsuranceApp.Domain.Results;

namespace InsuranceApp.UnitTests.Domain.Results;

public class ErrorTests
{
    [Fact]
    public void Unspecified_Defaults_CodeMessageAndKind()
    {
        var e = Error.Unspecified();

        e.Code.Should().Be("error.unspecified");
        e.Message.Should().Be("An unspecified error occurred.");
        e.Kind.Should().Be(ErrorKind.Failure);
    }

    [Fact]
    public void Unspecified_AllowsCustomMessage()
    {
        var e = Error.Unspecified("x");

        e.Message.Should().Be("x");
    }

    [Fact]
    public void Validation_NotFound_Conflict_CreateExpectedKindsAndCodes()
    {
        var v = Error.Validation("m");
        var n = Error.NotFound("m");
        var c = Error.Conflict("m");

        v.Kind.Should().Be(ErrorKind.Validation);
        v.Code.Should().Be("error.validation");

        n.Kind.Should().Be(ErrorKind.NotFound);
        n.Code.Should().Be("error.not_found");

        c.Kind.Should().Be(ErrorKind.Conflict);
        c.Code.Should().Be("error.conflict");
    }

    [Fact]
    public void Factories_AllowCustomCodeAndMetadata()
    {
        var md = new Dictionary<string, object?> { ["k"] = 1 };
        var e = Error.Validation("m", "custom.code", md);

        e.Code.Should().Be("custom.code");
        e.Metadata.Should().BeSameAs(md);
    }

    [Fact]
    public void ToString_PrintsCodeColonMessage()
    {
        var e = new Error("c", "m");

        e.ToString().Should().Be("c: m");
    }
}