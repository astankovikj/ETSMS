using Foundation.Application.DTOs;
using Foundation.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foundation.API.Endpoints;

public static class EmployeeDomainEndpoints
{
    public static void MapEmployeeDomainEndpoints(this WebApplication app)
    {
        app.MapPost("/employees/{employeeId:guid}/domains", [Authorize(Policy = "EmployeePolicy")] async (
            [FromServices] IEmployeeService service,
            Guid employeeId,
            [FromBody] AssignDomainRequest request,
            CancellationToken cancellationToken) =>
        {
            if (request.DomainId == Guid.Empty)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.DomainId)] = ["DomainId is required."]
                });
            }

            var (success, error) = await service.AssignDomainAsync(employeeId, request.DomainId, cancellationToken);

            if (!success && error == "Employee not found.")
                return Results.NotFound(new { message = error });

            if (!success && error == "Domain not found.")
                return Results.NotFound(new { message = error });

            if (!success)
                return Results.Conflict(new { message = error });

            return Results.NoContent();
        });

        app.MapDelete("/employees/{employeeId:guid}/domains/{domainId:guid}", [Authorize(Policy = "EmployeePolicy")] async (
            [FromServices] IEmployeeService service,
            Guid employeeId,
            Guid domainId,
            CancellationToken cancellationToken) =>
        {
            var (success, error) = await service.RemoveDomainAsync(employeeId, domainId, cancellationToken);

            if (!success && error == "Employee not found.")
                return Results.NotFound(new { message = error });

            if (!success)
                return Results.Conflict(new { message = error });

            return Results.NoContent();
        });
    }
}
