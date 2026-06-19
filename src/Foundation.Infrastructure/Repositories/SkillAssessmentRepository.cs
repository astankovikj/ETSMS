using System.Linq;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Repositories;

public sealed class SkillAssessmentRepository : ISkillAssessmentRepository
{
    private readonly FoundationDbContext _context;

    public SkillAssessmentRepository(FoundationDbContext context)
    {
        _context = context;
    }

    public async Task AddSnapshotAsync(SkillAssessmentSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _context.SkillAssessmentSnapshots.AddAsync(snapshot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<SkillAssessmentSnapshot?> GetLatestSnapshotAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.SkillAssessmentSnapshots
            .AsNoTracking()
            .Include(s => s.Entries)
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<SkillAssessmentSnapshot>> ListLatestSnapshotsAsync(CancellationToken cancellationToken = default)
    {
        var snapshots = await _context.SkillAssessmentSnapshots
            .AsNoTracking()
            .Include(s => s.Entries)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync(cancellationToken);

        return snapshots
            .GroupBy(s => s.EmployeeId)
            .Select(group => group.First())
            .ToList();
    }
}
