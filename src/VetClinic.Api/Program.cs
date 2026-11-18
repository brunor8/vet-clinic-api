using VetClinic.Api.Endpoints.Customers;
using VetClinic.Api.Extensions;
using VetClinic.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "Connection string 'Default' não configurada. Use user-secrets ou variável de ambiente.");

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.UseExceptionHandler();   // erros inesperados viram 500 em formato ProblemDetails

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();        // /openapi/v1.json
}

app.UseHttpsRedirection();

app.MapCustomerEndpoints();

app.Run();

// public partial class Program;   // permite WebApplicationFactory nos testes de integração