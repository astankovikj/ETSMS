using Foundation.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Foundation.API.Endpoints;

public static class ProfileEndpoints
{
    /// <summary>
    /// GET /api/profile/me
    ///
    /// Returns the full skill profile (technical skills grouped by category,
    /// domain expertise, certifications) for the authenticated employee.
    /// The employee identity is resolved exclusively from the JWT claims —
    /// no user-id query parameter is accepted (FR-6, NFR-6).
    /// </summary>
    public static async Task<IResult> GetMyProfileAsync(
        ClaimsPrincipal user,
        [FromServices] IProfileService profileService,
        CancellationToken cancellationToken)
    {
        // Resolve the employee ID from the JWT "sub" claim (standard) or
        // the "employeeId" custom claim if the token issuer uses one.
        var sub = user.FindFirstValue("employeeId")
               ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue("sub");

        if (sub is null || !Guid.TryParse(sub, out var employeeId))
        {
            return Results.Unauthorized();
        }

        var profile = await profileService.GetProfileAsync(employeeId, cancellationToken);

        return profile is null
            ? Results.NotFound()
            : Results.Ok(profile);
    }
}
