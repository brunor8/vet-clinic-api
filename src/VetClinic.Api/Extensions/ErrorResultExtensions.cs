using VetClinic.Application.Common.Results;

namespace VetClinic.Api.Extensions;

public static class ErrorResultExtensions
{
    public static IResult ToProblem(this IReadOnlyList<Error> errors)
    {
        var validation = errors.Where(e => e.Type == ErrorType.Validation).ToList();
        if (validation.Count > 0)
        {
            var grouped = validation
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            return Results.ValidationProblem(grouped);
        }

        var conflicts = errors.Where(e => e.Type == ErrorType.Conflict).ToList();
        if (conflicts.Count > 0)
        {
            return Results.Problem(
                title: "Data conflict",
                detail: string.Join(" ", conflicts.Select(e => e.Message)),
                statusCode: StatusCodes.Status409Conflict,
                extensions: new Dictionary<string, object?>
                {
                    ["errors"] = conflicts.Select(e => new { e.Code, e.Message }).ToArray()
                });
        }

        var notFound = errors.FirstOrDefault(e => e.Type == ErrorType.NotFound);
        if (notFound is not null)
        {
            return Results.Problem(
                title: "Não encontrado",
                detail: notFound.Message,
                statusCode: StatusCodes.Status404NotFound);
        }

        return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }
}