using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface IProfileRepository
{
    /// <summary>Returns all technical skills for the given employee, including their category.</summary>
    Task<IEnumerable<TechnicalSkill>> GetTechnicalSkillsAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>Returns all domain expertise entries for the given employee.</summary>
    Task<IEnumerable<DomainExpertise>> GetDomainExpertiseAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>Returns all certifications for the given employee.</summary>
    Task<IEnumerable<Certification>> GetCertificationsAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
