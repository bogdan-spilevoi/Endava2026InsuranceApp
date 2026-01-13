using System.Text.Json.Serialization;
using InsuranceApp.Application.Features.Clients.CreateClient;
using InsuranceApp.Domain.Enums;

namespace InsuranceApp.Api.Common.Requests.Clients;

public record CreateClientRequest(
    string Name,
    string IdentificationNumber,
    string Email,
    string PhoneNumber,
    string Street,
    string Number,
    string? Additional,
    [property: JsonRequired] ClientType ClientType) : IApiRequest<CreateClientCommand>
{
    public CreateClientCommand ToCommand()
    {
        return new CreateClientCommand(
            Name, 
            IdentificationNumber, 
            Email, 
            PhoneNumber, 
            Street, 
            Number, 
            Additional, 
            ClientType);
    }
};