using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface IProfileService
{
    /// <summary>
    /// Returns the full skill profile for the employee identified by <paramref name="email"/>.
    /// Returns <c>null</c> when no matching employee record exists.
    /// </summary>
    Task<ProfileDto?> GetMyProfileAsync(string email, CancellationToken cancellationToken = default);
}
