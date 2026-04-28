namespace Foundation.Domain.Entities;

public sealed class EmployeeDomain
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public Guid DomainId { get; set; }
    public Domain Domain { get; set; } = null!;
}
