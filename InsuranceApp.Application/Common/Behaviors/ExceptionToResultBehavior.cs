using MediatR;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Results;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Application.Common.Behaviors;

public sealed class ExceptionToResultBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (DomainException ex)
        {
            if (TryBuildFailure(Error.Validation(ex.Message), out var response))
                return response;

            throw;
        }
        catch (ValidationException ex)
        {
            if (TryBuildFailure(Error.Validation(ex.Message), out var response))
                return response;

            throw;
        }
        catch (KeyNotFoundException ex)
        {
            if (TryBuildFailure(Error.NotFound(ex.Message), out var response))
                return response;

            throw;
        }
        catch (Exception ex)
        {
            if (TryBuildFailure(Error.Unspecified(ex.Message), out var response))
                return response;

            throw;
        }
    }

    private static bool TryBuildFailure(Error error, out TResponse response)
    {
        var t = typeof(TResponse);

        if (t == typeof(Result))
        {
            response = (TResponse)(object)Result.Fail(error);
            return true;
        }

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failMethod = t.GetMethod("Fail", [typeof(Error)]);
            if (failMethod is not null)
            {
                response = (TResponse)failMethod.Invoke(null, [error])!;
                return true;
            }
        }

        response = default!;
        return false;
    }
}
