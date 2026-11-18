using VetClinic.Application.Common.Messaging;
using VetClinic.Application.Common.Results;
using VetClinic.Application.Customers.RegisterCustomer;

namespace VetClinic.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        services.AddScoped<ICommandHandler<RegisterCustomerCommand, Result<Guid>>, RegisterCustomerHandler>();

        return services;
    }
}