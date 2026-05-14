using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Repositories;

public sealed class ProfileRepository : IProfileRepository
{
    private readonly FoundationDbContext _context;

    public ProfileRepository(FoundationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TechnicalSkill>> GetTechnicalSkillsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TechnicalSkills
            .AsNoTracking()
            .Include(s => s.Category)
            .Where(s => s.EmployeeId == employeeId)
            .OrderBy(s => s.Category.Name)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DomainExpertise>> GetDomainExpertiseAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DomainExpertise
            .AsNoTracking()
            .Where(d => d.EmployeeId == employeeId)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Certification>> GetCertificationsAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Certifications
            .AsNoTracking()
            .Where(c => c.EmployeeId == employeeId)
            .OrderBy(c => c.ExpirationDate)
            .ToListAsync(cancellationToken);
    }
}
