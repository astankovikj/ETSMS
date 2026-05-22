using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Repositories;

public sealed class SkillRepository : ISkillRepository
{
    private readonly FoundationDbContext _context;

    public SkillRepository(FoundationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Skill>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .Where(s => EF.Functions.ILike(s.SkillName, $"%{query}%"))
            .OrderBy(s => s.SkillName)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<Skill?> GetByIdAsync(int skillId, CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SkillId == skillId, cancellationToken);
    }
}
