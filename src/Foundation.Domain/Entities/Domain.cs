namespace Foundation.Domain.Entities;

public sealed class Domain
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<EmployeeDomain> EmployeeDomains { get; set; } = new List<EmployeeDomain>();
}
