using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface IDomainService
{
    Task<IEnumerable<DomainDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<DomainDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(DomainDto? Domain, string? Error)> CreateAsync(CreateDomainRequest request, CancellationToken cancellationToken = default);
    Task<(DomainDto? Domain, string? Error)> UpdateAsync(Guid id, UpdateDomainRequest request, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error, int EmployeeCount)> DeleteAsync(Guid id, bool force = false, CancellationToken cancellationToken = default);
}
