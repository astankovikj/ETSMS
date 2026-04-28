using Foundation.Domain.Entities;

namespace Foundation.Application.Interfaces;

public interface IDomainRepository
{
    Task<IEnumerable<Domain>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Domain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Domain domain, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain domain, CancellationToken cancellationToken = default);
    Task DeleteAsync(Domain domain, CancellationToken cancellationToken = default);
    Task<int> CountEmployeesAsync(Guid domainId, CancellationToken cancellationToken = default);
}
