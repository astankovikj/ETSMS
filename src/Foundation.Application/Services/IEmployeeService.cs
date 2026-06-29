using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeProfileDto?> GetProfileAsync(Guid id, CancellationToken cancellationToken = default);
}
