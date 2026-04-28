using Foundation.Application.DTOs;
using Foundation.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Foundation.API.Endpoints;

public static class DomainEndpoints
{
    public static void MapDomainEndpoints(this WebApplication app)
    {
        app.MapGet("/domains", [Authorize(Policy = "EmployeePolicy")] async (
            [FromServices] IDomainService service,
            CancellationToken cancellationToken) =>
        {
            var domains = await service.ListAsync(cancellationToken);
            return Results.Ok(domains);
        });

        app.MapGet("/domains/{id:guid}", [Authorize(Policy = "EmployeePolicy")] async (
            [FromServices] IDomainService service,
            Guid id,
            CancellationToken cancellationToken) =>
        {
            var domain = await service.GetAsync(id, cancellationToken);
            return domain is null ? Results.NotFound() : Results.Ok(domain);
        });

        app.MapPost("/domains", [Authorize(Policy = "AdminPolicy")] async (
            [FromServices] IDomainService service,
            [FromBody] CreateDomainRequest request,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = ValidateCreateRequest(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var (domain, error) = await service.CreateAsync(request, cancellationToken);
            if (error is not null)
            {
                return Results.Conflict(new { message = error });
            }

            return Results.Created($"/domains/{domain!.Id}", domain);
        });

        app.MapPut("/domains/{id:guid}", [Authorize(Policy = "AdminPolicy")] async (
            [FromServices] IDomainService service,
            Guid id,
            [FromBody] UpdateDomainRequest request,
            CancellationToken cancellationToken) =>
        {
            var validationErrors = ValidateUpdateRequest(request);
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var (domain, error) = await service.UpdateAsync(id, request, cancellationToken);
            if (error == "Domain not found.")
            {
                return Results.NotFound();
            }
            if (error is not null)
            {
                return Results.Conflict(new { message = error });
            }

            return Results.Ok(domain);
        });

        app.MapDelete("/domains/{id:guid}", [Authorize(Policy = "AdminPolicy")] async (
            [FromServices] IDomainService service,
            Guid id,
            [FromQuery] bool force,
            CancellationToken cancellationToken) =>
        {
            var (success, error, employeeCount) = await service.DeleteAsync(id, force, cancellationToken);

            if (error == "Domain not found.")
            {
                return Results.NotFound();
            }

            if (!success && employeeCount > 0)
            {
                return Results.Conflict(new { message = error, employeeCount });
            }

            if (!success)
            {
                return Results.Problem(error);
            }

            return Results.NoContent();
        });
    }

    private static Dictionary<string, string[]> ValidateCreateRequest(CreateDomainRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors[nameof(request.Name)] = ["Domain Name is required."];
        else if (request.Name.Length > 50)
            errors[nameof(request.Name)] = ["Domain Name must not exceed 50 characters."];

        if (string.IsNullOrWhiteSpace(request.Type))
            errors[nameof(request.Type)] = ["Type is required."];
        else if (request.Type.Length > 50)
            errors[nameof(request.Type)] = ["Type must not exceed 50 characters."];

        if (request.Description is not null && request.Description.Length > 200)
            errors[nameof(request.Description)] = ["Description must not exceed 200 characters."];

        return errors;
    }

    private static Dictionary<string, string[]> ValidateUpdateRequest(UpdateDomainRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors[nameof(request.Name)] = ["Domain Name is required."];
        else if (request.Name.Length > 50)
            errors[nameof(request.Name)] = ["Domain Name must not exceed 50 characters."];

        if (string.IsNullOrWhiteSpace(request.Type))
            errors[nameof(request.Type)] = ["Type is required."];
        else if (request.Type.Length > 50)
            errors[nameof(request.Type)] = ["Type must not exceed 50 characters."];

        if (request.Description is not null && request.Description.Length > 200)
            errors[nameof(request.Description)] = ["Description must not exceed 200 characters."];

        return errors;
    }
}
