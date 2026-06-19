using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface ISkillAssessmentService
{
    Task<SkillAssessmentSnapshotDto> CreateAsync(Guid employeeId, CreateSkillAssessmentRequest request, CancellationToken cancellationToken = default);
    Task<SkillAssessmentSnapshotDto> GetLatestSnapshotAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SkillAssessmentSnapshotDto>> ListLatestByEmployeesAsync(CancellationToken cancellationToken = default);
}
