namespace VetClinic.Api.Endpoints.Customers;

public sealed record CreateCustomerRequest(
    string? Name,
    string? Cpf,
    string? Email,
    string? Phone);