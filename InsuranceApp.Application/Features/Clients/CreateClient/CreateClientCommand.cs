using InsuranceApp.Domain.Enums;
using InsuranceApp.Domain.Results;
using MediatR;

namespace InsuranceApp.Application.Features.Clients.CreateClient;

public record CreateClientCommand(
    string Name, 
    string IdentificationNumber, 
    string Email, 
    string PhoneNumber, 
    string Street, 
    string Number, 
    string? Additional, 
    ClientType ClientType) : IRequest<Result<Guid>>;