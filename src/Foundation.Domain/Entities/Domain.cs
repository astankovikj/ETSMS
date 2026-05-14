namespace Foundation.Domain.Entities;

public sealed class Domain
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<EmployeeDomain> EmployeeDomains { get; set; } = new List<EmployeeDomain>();
}
