namespace Foundation.Domain.Entities;

public sealed class DomainExpertise
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Employee Employee { get; set; } = null!;
}
