using System;

namespace Foundation.Domain.Entities;

public sealed class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
    public ICollection<EmployeeDomain> EmployeeDomains { get; set; } = new List<EmployeeDomain>();
    public ICollection<EmployeeCertification> EmployeeCertifications { get; set; } = new List<EmployeeCertification>();
}
