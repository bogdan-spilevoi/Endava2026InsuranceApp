using FluentAssertions;
using InsuranceApp.Domain.Results;

namespace InsuranceApp.UnitTests.Domain.Results;

public class ResultOfTTests
{
    [Fact]
    public void Ok_WithNullValue_Throws()
    {
        Action act = () => Result<string>.Ok(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ok_WithNonNullValue_IsSuccess_And_ValueAccessible()
    {
        var r = Result<string>.Ok("x");

        r.IsSuccess.Should().BeTrue();
        r.IsFailure.Should().BeFalse();
        r.Errors.Should().NotBeNull().And.BeEmpty();
        r.Value.Should().Be("x");
    }

    [Fact]
    public void OkNullable_AllowsNullValue_IsSuccess_And_ValueIsNull()
    {
        var r = Result<string>.OkNullable(null);

        r.IsSuccess.Should().BeTrue();
        r.IsFailure.Should().BeFalse();
        r.Errors.Should().BeEmpty();
        r.Value.Should().BeNull();
    }

    [Fact]
    public void Value_OnFailure_Throws()
    {
        var r = Result<int>.Fail(Error.Validation("bad"));

        Action act = () => _ = r.Value;

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*Cannot access Value when the result is a failure*");
    }

    [Fact]
    public void Fail_WithSingleError_IsFailure_And_PreservesError()
    {
        var err = Error.NotFound("missing");
        var r = Result<int>.Fail(err);

        r.IsSuccess.Should().BeFalse();
        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Should().BeSameAs(err);
    }

    [Fact]
    public void Fail_Params_WithEmptyArray_NormalizesToUnspecified()
    {
        var r = Result<int>.Fail(Array.Empty<Error>());

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Fail_Params_WithNullArray_NormalizesToUnspecified()
    {
        Error[]? errors = null;

        var r = Result<int>.Fail(errors!);

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Fail_List_WithEmptyList_NormalizesToUnspecified()
    {
        var r = Result<int>.Fail(new List<Error>());

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Fail_List_WithNullList_NormalizesToUnspecified()
    {
        IReadOnlyList<Error>? errors = null;

        var r = Result<int>.Fail(errors!);

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Map_WhenMapperNull_Throws()
    {
        var r = Result<int>.Ok(1);

        Action act = () => r.Map<string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Map_OnSuccess_MapsValue_UsingOkNullable()
    {
        var r = Result<int>.Ok(2);

        var mapped = r.Map(x => (x * 3).ToString());

        mapped.IsSuccess.Should().BeTrue();
        mapped.Errors.Should().BeEmpty();
        mapped.Value.Should().Be("6");
    }

    [Fact]
    public void Map_OnSuccess_AllowsMapperReturningNull_BecauseOkNullable()
    {
        var r = Result<int>.Ok(2);

        var mapped = r.Map<string>(_ => null!);

        mapped.IsSuccess.Should().BeTrue();
        mapped.Errors.Should().BeEmpty();
        mapped.Value.Should().BeNull();
    }

    [Fact]
    public void Map_OnFailure_DoesNotInvokeMapper_AndPropagatesErrors()
    {
        var err = Error.Conflict("boom");
        var r = Result<int>.Fail(err);
        var called = false;

        var mapped = r.Map<string>(_ =>
        {
            called = true;
            return "x";
        });

        mapped.IsFailure.Should().BeTrue();
        called.Should().BeFalse();
        mapped.Errors.Should().ContainSingle();
        mapped.Errors[0].Should().BeSameAs(err);
    }

    [Fact]
    public void Bind_WhenBinderNull_Throws()
    {
        var r = Result<int>.Ok(1);

        Action act = () => r.Bind<string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Bind_OnSuccess_InvokesBinder()
    {
        var r = Result<int>.Ok(5);

        var bound = r.Bind(x => Result<string>.Ok((x + 1).ToString()));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("6");
    }

    [Fact]
    public void Bind_OnFailure_DoesNotInvokeBinder_AndPropagatesErrors()
    {
        var err = Error.Validation("bad");
        var r = Result<int>.Fail(err);
        var called = false;

        var bound = r.Bind<string>(_ =>
        {
            called = true;
            return Result<string>.Ok("nope");
        });

        bound.IsFailure.Should().BeTrue();
        called.Should().BeFalse();
        bound.Errors.Should().ContainSingle();
        bound.Errors[0].Should().BeSameAs(err);
    }

    [Fact]
    public void Match_WhenOnSuccessNull_Throws()
    {
        var r = Result<int>.Ok(1);

        Action act = () => r.Match<string>(null!, _ => "x");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_WhenOnFailureNull_Throws()
    {
        var r = Result<int>.Ok(1);

        Action act = () => r.Match<string>(_ => "x", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_OnSuccess_CallsOnSuccess()
    {
        var r = Result<int>.Ok(7);

        var res = r.Match(
            onSuccess: v => $"S:{v}",
            onFailure: _ => "F");

        res.Should().Be("S:7");
    }

    [Fact]
    public void Match_OnFailure_CallsOnFailure()
    {
        var err = Error.NotFound("missing");
        var r = Result<int>.Fail(err);

        var res = r.Match(
            onSuccess: _ => "S",
            onFailure: errs => $"F:{errs.Count}");

        res.Should().Be("F:1");
    }

    [Fact]
    public void Tap_WhenActionNull_Throws()
    {
        var r = Result<int>.Ok(1);

        Action act = () => r.Tap(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Tap_OnSuccess_InvokesAction_AndReturnsResult()
    {
        var r = Result<int>.Ok(10);
        var observed = 0;

        var r2 = r.Tap(v => observed = v);

        observed.Should().Be(10);
        r2.IsSuccess.Should().BeTrue();
        r2.Value.Should().Be(10);
    }

    [Fact]
    public void Tap_OnFailure_DoesNotInvokeAction()
    {
        var r = Result<int>.Fail(Error.Validation("bad"));
        var called = false;

        var r2 = r.Tap(_ => called = true);

        called.Should().BeFalse();
        r2.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TapFailure_WhenActionNull_Throws()
    {
        var r = Result<int>.Fail(Error.Validation("bad"));

        Action act = () => r.TapFailure(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TapFailure_OnFailure_InvokesAction()
    {
        var err = Error.Validation("bad");
        var r = Result<int>.Fail(err);
        var count = 0;

        var r2 = r.TapFailure(errs => count = errs.Count);

        count.Should().Be(1);
        r2.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TapFailure_OnSuccess_DoesNotInvokeAction()
    {
        var r = Result<int>.Ok(1);
        var called = false;

        var r2 = r.TapFailure(_ => called = true);

        called.Should().BeFalse();
        r2.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryGetValue_OnSuccess_ReturnsTrue_AndOutputsValue()
    {
        var r = Result<string>.Ok("abc");

        var ok = r.TryGetValue(out var v);

        ok.Should().BeTrue();
        v.Should().Be("abc");
    }

    [Fact]
    public void TryGetValue_OnFailure_ReturnsFalse_AndOutputsDefault()
    {
        var r = Result<string>.Fail(Error.Validation("bad"));

        var ok = r.TryGetValue(out var v);

        ok.Should().BeFalse();
        v.Should().BeNull();
    }

    [Fact]
    public void ToString_OnSuccess_WithNullValue_PrintsOkNull()
    {
        var r = Result<string>.OkNullable(null);

        r.ToString().Should().Be("Ok(null)");
    }

    [Fact]
    public void ToString_OnSuccess_WithValue_PrintsOkValue()
    {
        var r = Result<int>.Ok(42);

        r.ToString().Should().Be("Ok(42)");
    }

    [Fact]
    public void ToString_OnFailure_PrintsFailAndErrors()
    {
        var r = Result<int>.Fail(Error.Validation("bad", "error.validation"));

        r.ToString().Should().StartWith("Fail(");
        r.ToString().Should().Contain("error.validation: bad");
    }

    [Fact]
    public void ImplicitOperator_FromValue_CreatesOk()
    {
        Result<int> r = 123;

        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(123);
        r.Errors.Should().BeEmpty();
    }
}

public class ResultNonGenericTests
{
    [Fact]
    public void Ok_IsSuccess_AndHasNoErrors()
    {
        var r = Result.Ok();

        r.IsSuccess.Should().BeTrue();
        r.IsFailure.Should().BeFalse();
        r.Errors.Should().BeEmpty();
        r.ToString().Should().Be("Ok");
    }

    [Fact]
    public void Fail_WithSingleError_IsFailure_AndPreservesError()
    {
        var err = Error.Conflict("conflict");
        var r = Result.Fail(err);

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Should().BeSameAs(err);
        r.ToString().Should().Contain("Fail(");
    }

    [Fact]
    public void Fail_Params_WithEmptyArray_NormalizesToUnspecified()
    {
        var r = Result.Fail(Array.Empty<Error>());

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Fail_Params_WithNullArray_NormalizesToUnspecified()
    {
        Error[]? errors = null;

        var r = Result.Fail(errors!);

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Fail_List_WithEmptyList_NormalizesToUnspecified()
    {
        var r = Result.Fail(new List<Error>());

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void Fail_List_WithNullList_NormalizesToUnspecified()
    {
        IReadOnlyList<Error>? errors = null;

        var r = Result.Fail(errors!);

        r.IsFailure.Should().BeTrue();
        r.Errors.Should().ContainSingle();
        r.Errors[0].Code.Should().Be("error.unspecified");
    }

    [Fact]
    public void ToResult_OnSuccess_CreatesOkResultOfT()
    {
        var r = Result.Ok();

        var rt = r.ToResult(10);

        rt.IsSuccess.Should().BeTrue();
        rt.Value.Should().Be(10);
        rt.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ToResult_OnFailure_PropagatesErrors()
    {
        var err = Error.NotFound("missing");
        var r = Result.Fail(err);

        var rt = r.ToResult(10);

        rt.IsFailure.Should().BeTrue();
        rt.Errors.Should().ContainSingle();
        rt.Errors[0].Should().BeSameAs(err);
    }

    [Fact]
    public void Match_WhenOnSuccessNull_Throws()
    {
        var r = Result.Ok();

        Action act = () => r.Match<string>(null!, _ => "x");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_WhenOnFailureNull_Throws()
    {
        var r = Result.Ok();

        Action act = () => r.Match<string>(() => "x", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Match_OnSuccess_CallsOnSuccess()
    {
        var r = Result.Ok();

        var res = r.Match(
            onSuccess: () => "S",
            onFailure: _ => "F");

        res.Should().Be("S");
    }

    [Fact]
    public void Match_OnFailure_CallsOnFailure()
    {
        var r = Result.Fail(Error.Validation("bad"));

        var res = r.Match(
            onSuccess: () => "S",
            onFailure: errs => $"F:{errs.Count}");

        res.Should().Be("F:1");
    }

    [Fact]
    public void ToString_OnFailure_PrintsFailAndErrors()
    {
        var r = Result.Fail(Error.Validation("bad", "error.validation"));

        r.ToString().Should().StartWith("Fail(");
        r.ToString().Should().Contain("error.validation: bad");
    }
}
