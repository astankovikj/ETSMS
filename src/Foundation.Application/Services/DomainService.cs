using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;

namespace Foundation.Application.Services;

public sealed class DomainService : IDomainService
{
    private readonly IDomainRepository _repository;

    public DomainService(IDomainRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DomainDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var domains = await _repository.GetAllAsync(cancellationToken);
        return domains.Select(ToDto);
    }

    public async Task<DomainDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var domain = await _repository.GetByIdAsync(id, cancellationToken);
        return domain is null ? null : ToDto(domain);
    }

    public async Task<(DomainDto? Domain, string? Error)> CreateAsync(
        CreateDomainRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
        {
            return (null, $"A domain with the name '{request.Name}' already exists.");
        }

        var domain = new Domain
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Type = request.Type.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(domain, cancellationToken);
        return (ToDto(domain), null);
    }

    public async Task<(DomainDto? Domain, string? Error)> UpdateAsync(
        Guid id,
        UpdateDomainRequest request,
        CancellationToken cancellationToken = default)
    {
        var domain = await _repository.GetByIdAsync(id, cancellationToken);
        if (domain is null)
        {
            return (null, "Domain not found.");
        }

        // Uniqueness check — ignore the current record itself
        var existing = await _repository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null && existing.Id != id)
        {
            return (null, $"A domain with the name '{request.Name}' already exists.");
        }

        domain.Name = request.Name.Trim();
        domain.Type = request.Type.Trim();
        domain.Description = request.Description?.Trim();

        await _repository.UpdateAsync(domain, cancellationToken);
        return (ToDto(domain), null);
    }

    public async Task<(bool Success, string? Error, int EmployeeCount)> DeleteAsync(
        Guid id,
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        var domain = await _repository.GetByIdAsync(id, cancellationToken);
        if (domain is null)
        {
            return (false, "Domain not found.", 0);
        }

        var employeeCount = await _repository.CountEmployeesAsync(id, cancellationToken);
        if (employeeCount > 0 && !force)
        {
            return (false, $"This domain is associated with {employeeCount} employee(s). Pass force=true to confirm deletion.", employeeCount);
        }

        await _repository.DeleteAsync(domain, cancellationToken);
        return (true, null, employeeCount);
    }

    private static DomainDto ToDto(Domain domain) => new()
    {
        Id = domain.Id,
        Name = domain.Name,
        Type = domain.Type,
        Description = domain.Description
    };
}
