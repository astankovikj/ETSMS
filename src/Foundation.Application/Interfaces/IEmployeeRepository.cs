using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetAllByDomainIdsAsync(IEnumerable<Guid> domainIds, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    Task AssignDomainAsync(Guid employeeId, Guid domainId, CancellationToken cancellationToken = default);
    Task RemoveDomainAsync(Guid employeeId, Guid domainId, CancellationToken cancellationToken = default);
}
