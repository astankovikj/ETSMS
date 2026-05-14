using Foundation.API.Endpoints;
using Microsoft.AspNetCore.Authorization;

namespace Foundation.API.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/profile/me",
            [Authorize(Policy = "EmployeePolicy")] (
                System.Security.Claims.ClaimsPrincipal user,
                [Microsoft.AspNetCore.Mvc.FromServices] Foundation.Application.Services.IProfileService profileService,
                CancellationToken cancellationToken) =>
                ProfileEndpoints.GetMyProfileAsync(user, profileService, cancellationToken))
           .WithName("GetMyProfile")
           .WithTags("Profile")
           .Produces(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized)
           .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
