using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Repositories;

public sealed class DomainRepository : IDomainRepository
{
    private readonly FoundationDbContext _context;

    public DomainRepository(FoundationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Domain>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Domains
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Domains
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Domain?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Domains
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task AddAsync(Domain domain, CancellationToken cancellationToken = default)
    {
        await _context.Domains.AddAsync(domain, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Domain domain, CancellationToken cancellationToken = default)
    {
        _context.Domains.Update(domain);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Domain domain, CancellationToken cancellationToken = default)
    {
        _context.Domains.Remove(domain);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountEmployeesAsync(Guid domainId, CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeDomains
            .CountAsync(ed => ed.DomainId == domainId, cancellationToken);
    }
}
