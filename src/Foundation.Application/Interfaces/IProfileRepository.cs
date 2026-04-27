using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface IProfileRepository
{
    Task<IEnumerable<TechnicalSkill>> GetTechnicalSkillsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DomainExpertise>> GetDomainExpertiseAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Certification>> GetCertificationsAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
