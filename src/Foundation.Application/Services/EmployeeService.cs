using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;

namespace Foundation.Application.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IDomainRepository _domainRepository;

    public EmployeeService(IEmployeeRepository repository, IDomainRepository domainRepository)
    {
        _repository = repository;
        _domainRepository = domainRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);
        return employees.Select(ToDto);
    }

    public async Task<IEnumerable<EmployeeDto>> ListByDomainIdsAsync(
        IEnumerable<Guid> domainIds,
        CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllByDomainIdsAsync(domainIds, cancellationToken);
        return employees.Select(ToDto);
    }

    public async Task<EmployeeDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        return ToDto(employee);
    }

    public async Task<(bool Success, string? Error)> AssignDomainAsync(
        Guid employeeId,
        Guid domainId,
        CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return (false, "Employee not found.");
        }

        var domain = await _domainRepository.GetByIdAsync(domainId, cancellationToken);
        if (domain is null)
        {
            return (false, "Domain not found.");
        }

        var alreadyAssigned = employee.EmployeeDomains.Any(ed => ed.DomainId == domainId);
        if (alreadyAssigned)
        {
            return (false, "Domain is already assigned to this employee.");
        }

        await _repository.AssignDomainAsync(employeeId, domainId, cancellationToken);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> RemoveDomainAsync(
        Guid employeeId,
        Guid domainId,
        CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return (false, "Employee not found.");
        }

        var assigned = employee.EmployeeDomains.Any(ed => ed.DomainId == domainId);
        if (!assigned)
        {
            return (false, "Domain is not assigned to this employee.");
        }

        await _repository.RemoveDomainAsync(employeeId, domainId, cancellationToken);
        return (true, null);
    }

    private static EmployeeDto ToDto(Employee employee) => new()
    {
        Id = employee.Id,
        FullName = employee.FullName,
        Email = employee.Email,
        Role = employee.Role,
        Domains = employee.EmployeeDomains
            .Select(ed => new DomainDto
            {
                Id = ed.Domain.Id,
                Name = ed.Domain.Name,
                Type = ed.Domain.Type,
                Description = ed.Domain.Description
            })
            .ToList()
    };
}
