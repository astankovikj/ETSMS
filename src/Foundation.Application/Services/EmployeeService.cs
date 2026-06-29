using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;

namespace Foundation.Application.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmployeeDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetAllAsync(cancellationToken);

        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FullName = e.FullName,
            Email = e.Email,
            Role = e.Role
        });
    }

    public async Task<EmployeeDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Role = employee.Role
        };
    }

    public async Task<EmployeeProfileDto?> GetProfileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetProfileAsync(id, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        return new EmployeeProfileDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Role = employee.Role,
            Department = employee.Department,
            JobTitle = employee.JobTitle,
            PhoneNumber = employee.PhoneNumber,
            ProfilePictureUrl = employee.ProfilePictureUrl,
            Bio = employee.Bio,
            Location = employee.Location
        };
    }
}
