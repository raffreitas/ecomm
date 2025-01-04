using Ecomm.Customers.Api.Requests;
using Ecomm.Customers.Api.Services;

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
        ICustomerService customerService,
        CancellationToken cancellationToken)
    {
        await customerService.CreateAsync(customerRequest, cancellationToken);
        return Results.Created();
    }
}