using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface ISkillAssessmentRepository
{
    Task AddSnapshotAsync(SkillAssessmentSnapshot snapshot, CancellationToken cancellationToken = default);
    Task<SkillAssessmentSnapshot?> GetLatestSnapshotAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SkillAssessmentSnapshot>> ListLatestSnapshotsAsync(CancellationToken cancellationToken = default);
}
