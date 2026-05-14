using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface IProfileRepository
{
    /// <summary>
    /// Returns the employee whose e-mail matches the authenticated user's identity claim,
    /// including all related skill, domain, and certification data.
    /// Returns <c>null</c> when no matching employee record exists.
    /// </summary>
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
