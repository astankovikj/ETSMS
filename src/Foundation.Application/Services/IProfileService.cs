using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface IProfileService
{
    /// <summary>
    /// Returns the full profile for the authenticated employee identified by
    /// <paramref name="employeeId"/> (resolved from JWT claims by the caller).
    /// Returns <c>null</c> when no employee record exists for the given ID.
    /// </summary>
    Task<ProfileDto?> GetProfileAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
