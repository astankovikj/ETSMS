using Foundation.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Foundation.API.Endpoints;

public static class SkillEndpoints
{
    public static IEndpointRouteBuilder MapSkillEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/skills", [Authorize(Policy = "EmployeePolicy")] async (
            string? search,
            ISkillService skillService,
            CancellationToken cancellationToken) =>
        {
            var query = search ?? string.Empty;
            var skills = await skillService.SearchSkillsAsync(query, cancellationToken);
            return Results.Ok(skills);
        });

        return app;
    }
}
