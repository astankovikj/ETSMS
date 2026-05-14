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

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                    .ThenInclude(s => s.Category)
            .Include(e => e.EmployeeDomains)
                .ThenInclude(ed => ed.Domain)
            .Include(e => e.EmployeeCertifications)
                .ThenInclude(ec => ec.Certification)
            .FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }
}
