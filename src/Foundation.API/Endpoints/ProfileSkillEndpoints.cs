using Foundation.Application.DTOs;
using Foundation.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Foundation.API.Endpoints;

public static class ProfileSkillEndpoints
{
    public static IEndpointRouteBuilder MapProfileSkillEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profile/skills", [Authorize(Policy = "EmployeePolicy")] async (
            [FromBody] AddEmployeeSkillDto dto,
            ISkillService skillService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            if (dto.SkillId <= 0 || dto.ProficiencyLevel < 1 || dto.ProficiencyLevel > 5)
            {
                return Results.BadRequest(new { error = "skillId and proficiencyLevel (1–5) are required." });
            }

            var employeeIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("oid")
                ?? user.FindFirstValue("sub");

            if (employeeIdClaim is null || !Guid.TryParse(employeeIdClaim, out var employeeId))
            {
                return Results.Unauthorized();
            }

            var result = await skillService.AddEmployeeSkillAsync(employeeId, dto, cancellationToken);

            return result.Status switch
            {
                AddEmployeeSkillResultStatus.Created =>
                    Results.Created($"/profile/skills/{result.EmployeeSkill!.SkillId}", result.EmployeeSkill),

                AddEmployeeSkillResultStatus.Duplicate =>
                    Results.Conflict(new { error = result.ErrorMessage }),

                AddEmployeeSkillResultStatus.SkillNotFound =>
                    Results.BadRequest(new { error = result.ErrorMessage }),

                AddEmployeeSkillResultStatus.ValidationError =>
                    Results.BadRequest(new { error = result.ErrorMessage }),

                _ => Results.StatusCode(500)
            };
        });

        return app;
    }
}
