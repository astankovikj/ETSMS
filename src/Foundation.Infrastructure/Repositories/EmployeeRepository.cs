using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly FoundationDbContext _context;

    public EmployeeRepository(FoundationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeDomains)
                .ThenInclude(ed => ed.Domain)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetAllByDomainIdsAsync(
        IEnumerable<Guid> domainIds,
        CancellationToken cancellationToken = default)
    {
        var domainIdList = domainIds.ToList();

        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeDomains)
                .ThenInclude(ed => ed.Domain)
            .Where(e => e.EmployeeDomains.Any(ed => domainIdList.Contains(ed.DomainId)))
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeDomains)
                .ThenInclude(ed => ed.Domain)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignDomainAsync(
        Guid employeeId,
        Guid domainId,
        CancellationToken cancellationToken = default)
    {
        var employeeDomain = new EmployeeDomain
        {
            EmployeeId = employeeId,
            DomainId = domainId
        };

        await _context.EmployeeDomains.AddAsync(employeeDomain, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveDomainAsync(
        Guid employeeId,
        Guid domainId,
        CancellationToken cancellationToken = default)
    {
        var employeeDomain = await _context.EmployeeDomains
            .FirstOrDefaultAsync(
                ed => ed.EmployeeId == employeeId && ed.DomainId == domainId,
                cancellationToken);

        if (employeeDomain is not null)
        {
            _context.EmployeeDomains.Remove(employeeDomain);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
