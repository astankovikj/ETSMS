using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface IProfileService
{
    Task<EmployeeProfileDto?> GetMyProfileAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
