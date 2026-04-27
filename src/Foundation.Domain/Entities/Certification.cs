namespace Foundation.Domain.Entities;

public sealed class Certification
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    // Navigation property
    public Employee? Employee { get; set; }
}
