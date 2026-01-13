using InsuranceApp.Api.Common;
using InsuranceApp.Api.Common.Requests.Clients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.Api.Controllers;

[ApiController]
[Route("api/brokers/clients")]
public class ClientController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(request.ToCommand(), ct);
        return this.ToActionResult(result);
    }
}