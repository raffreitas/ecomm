using Ecomm.Customers.Api.Features.CreateCustomer;
using Ecomm.Customers.Api.Requests;

namespace Ecomm.Customers.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customers");
        group.MapPost("", CreateCustomer);
    }

    private static async Task<IResult> CreateCustomer(
        CreateCustomerRequest customerRequest,
        CreateCustomerHandler handler,
        CancellationToken cancellationToken)
    {
        var id = await handler.ExecuteAsync(customerRequest, cancellationToken);
        return Results.Created($"/api/customers/{id}", new { id });
    }
}
