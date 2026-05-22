using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface ISkillRepository
{
    Task<IEnumerable<Skill>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<Skill?> GetByIdAsync(int skillId, CancellationToken cancellationToken = default);
}
