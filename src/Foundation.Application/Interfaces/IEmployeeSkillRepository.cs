using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface IEmployeeSkillRepository
{
    Task<bool> ExistsAsync(Guid employeeId, int skillId, CancellationToken cancellationToken = default);
    Task AddAsync(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default);
}
