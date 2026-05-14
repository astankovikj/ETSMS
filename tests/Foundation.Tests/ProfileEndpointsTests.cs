using System.Security.Claims;
using FluentAssertions;
using Foundation.API.Endpoints;
using Foundation.Application.DTOs;
using Foundation.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Xunit;

namespace Foundation.Tests;

/// <summary>
/// Unit tests for <see cref="ProfileEndpoints.GetMyProfileAsync"/> — covers:
/// - 401 when the JWT has no "sub" / "employeeId" / NameIdentifier claim (AC-6, AC-7)
/// - 401 when the sub claim is not a valid GUID (AC-7)
/// - 404 when the employee record does not exist
/// - 200 OK with the full profile DTO when the employee exists (AC-1)
/// </summary>
public sealed class ProfileEndpointsTests
{
    private readonly IProfileService _profileService = Substitute.For<IProfileService>();

    // ── AC-7: missing sub claim → 401 ────────────────────────────────────────

    [Fact]
    public async Task GetMyProfileAsync_Returns401_WhenNoSubClaim()
    {
        var user = MakeClaimsPrincipal(); // no relevant claims

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    // ── AC-7: sub claim is not a valid GUID → 401 ────────────────────────────

    [Fact]
    public async Task GetMyProfileAsync_Returns401_WhenSubClaimIsNotAGuid()
    {
        var user = MakeClaimsPrincipal(new Claim("sub", "not-a-guid"));

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public async Task GetMyProfileAsync_Returns401_WhenNameIdentifierClaimIsNotAGuid()
    {
        var user = MakeClaimsPrincipal(new Claim(ClaimTypes.NameIdentifier, "bad-value"));

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedHttpResult>();
    }

    // ── FR-6: employee identity resolved from JWT claims, not query param ────
    //    (the endpoint signature has no id parameter — verified at compile time)

    // ── 404 when the service returns null (employee record not found) ─────────

    [Fact]
    public async Task GetMyProfileAsync_Returns404_WhenEmployeeNotFound()
    {
        var employeeId = Guid.NewGuid();
        var user = MakeClaimsPrincipal(new Claim("sub", employeeId.ToString()));

        _profileService.GetProfileAsync(employeeId, Arg.Any<CancellationToken>())
                       .Returns((ProfileDto?)null);

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        result.Should().BeOfType<NotFound>();
    }

    // ── AC-1: valid token with "sub" claim → 200 OK with profile ─────────────

    [Fact]
    public async Task GetMyProfileAsync_Returns200_WhenEmployeeFoundViaSubClaim()
    {
        var employeeId = Guid.NewGuid();
        var user = MakeClaimsPrincipal(new Claim("sub", employeeId.ToString()));
        var profile = BuildSampleProfile(employeeId);

        _profileService.GetProfileAsync(employeeId, Arg.Any<CancellationToken>())
                       .Returns(profile);

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<ProfileDto>>().Subject;
        okResult.Value.Should().BeSameAs(profile);
    }

    // ── AC-1: valid token with "employeeId" custom claim → 200 OK ────────────

    [Fact]
    public async Task GetMyProfileAsync_Returns200_WhenEmployeeFoundViaEmployeeIdClaim()
    {
        var employeeId = Guid.NewGuid();
        var user = MakeClaimsPrincipal(new Claim("employeeId", employeeId.ToString()));
        var profile = BuildSampleProfile(employeeId);

        _profileService.GetProfileAsync(employeeId, Arg.Any<CancellationToken>())
                       .Returns(profile);

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<ProfileDto>>().Subject;
        okResult.Value.Should().BeSameAs(profile);
    }

    // ── AC-1: valid token with NameIdentifier claim → 200 OK ─────────────────

    [Fact]
    public async Task GetMyProfileAsync_Returns200_WhenEmployeeFoundViaNameIdentifierClaim()
    {
        var employeeId = Guid.NewGuid();
        var user = MakeClaimsPrincipal(new Claim(ClaimTypes.NameIdentifier, employeeId.ToString()));
        var profile = BuildSampleProfile(employeeId);

        _profileService.GetProfileAsync(employeeId, Arg.Any<CancellationToken>())
                       .Returns(profile);

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<ProfileDto>>().Subject;
        okResult.Value.Should().BeSameAs(profile);
    }

    // ── "employeeId" claim takes priority over NameIdentifier ─────────────────

    [Fact]
    public async Task GetMyProfileAsync_PrefersEmployeeIdClaim_OverNameIdentifier()
    {
        var primaryId = Guid.NewGuid();
        var secondaryId = Guid.NewGuid();

        var user = MakeClaimsPrincipal(
            new Claim("employeeId", primaryId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, secondaryId.ToString()));

        var profile = BuildSampleProfile(primaryId);

        _profileService.GetProfileAsync(primaryId, Arg.Any<CancellationToken>())
                       .Returns(profile);

        var result = await ProfileEndpoints.GetMyProfileAsync(user, _profileService, CancellationToken.None);

        result.Should().BeOfType<Ok<ProfileDto>>();

        // Should NOT have called the service with the secondary ID
        await _profileService.DidNotReceive().GetProfileAsync(secondaryId, Arg.Any<CancellationToken>());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static ClaimsPrincipal MakeClaimsPrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }

    private static ProfileDto BuildSampleProfile(Guid employeeId) => new()
    {
        Employee = new EmployeeDto
        {
            Id = employeeId,
            FullName = "Sarah Chen",
            Email = "sarah.chen@corp.com",
            Role = "Employee"
        },
        TechnicalSkills = [],
        DomainExpertise = [],
        Certifications = []
    };
}
