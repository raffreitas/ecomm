using Ecomm.Orders.Application.Customers.Register;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Orders.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCustomerCommand registerCustomerCommand)
    {
        await _sender.Send(registerCustomerCommand);
        return Ok();
    }
}