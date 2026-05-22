using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Repositories;

public sealed class EmployeeSkillRepository : IEmployeeSkillRepository
{
    private readonly FoundationDbContext _context;

    public EmployeeSkillRepository(FoundationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid employeeId, int skillId, CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeSkills
            .AsNoTracking()
            .AnyAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId, cancellationToken);
    }

    public async Task AddAsync(EmployeeSkill employeeSkill, CancellationToken cancellationToken = default)
    {
        await _context.EmployeeSkills.AddAsync(employeeSkill, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
