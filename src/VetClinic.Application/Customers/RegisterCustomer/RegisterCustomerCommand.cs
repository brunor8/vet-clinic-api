using VetClinic.Application.Common.Messaging;
using VetClinic.Application.Common.Results;

namespace VetClinic.Application.Customers.RegisterCustomer;

public sealed record RegisterCustomerCommand(
    string? Name,
    string? Cpf,
    string? Email,
    string? Phone) : ICommand<Result<Guid>>;