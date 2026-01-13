using InsuranceApp.Domain.Clients;
using InsuranceApp.Domain.Common;
using InsuranceApp.Domain.Repositories;
using InsuranceApp.Domain.Results;
using MediatR;

namespace InsuranceApp.Application.Features.Clients.CreateClient;

public class CreateClientCommandHandler(IClientRepository _clientRepo) : IRequestHandler<CreateClientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClientCommand command, CancellationToken cancellationToken)
    {

        var newClientId = Guid.NewGuid();
        var newClient = new Client(
            newClientId,
            command.ClientType,
            command.Name,
            command.IdentificationNumber,
            new(command.Email),
            new(command.PhoneNumber),
            new(command.Street, command.Number, command.Additional)
        );
        await _clientRepo.AddAsync(newClient, cancellationToken);
        return Result<Guid>.Ok(newClientId);
  
    }
}