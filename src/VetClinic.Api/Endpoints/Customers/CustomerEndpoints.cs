using VetClinic.Api.Extensions;
using VetClinic.Application.Common.Messaging;
using VetClinic.Application.Customers.RegisterCustomer;

namespace VetClinic.Api.Endpoints.Customers;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapPost("/", Register)
            .Produces<CreateCustomerResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<IResult> Register(
        CreateCustomerRequest request,
        ICommandDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCustomerCommand(request.Name, request.Cpf, request.Email, request.Phone);

        var result = await dispatcher.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/customers/{result.Value}", new CreateCustomerResponse(result.Value))
            : result.Errors.ToProblem();
    }
}