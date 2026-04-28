using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeDto>> ListByDomainIdsAsync(IEnumerable<Guid> domainIds, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> AssignDomainAsync(Guid employeeId, Guid domainId, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> RemoveDomainAsync(Guid employeeId, Guid domainId, CancellationToken cancellationToken = default);
}
