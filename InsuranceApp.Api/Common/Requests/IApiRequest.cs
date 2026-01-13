namespace InsuranceApp.Api.Common.Requests;

public interface IApiRequest<out TCommand>
{
    TCommand ToCommand();
}