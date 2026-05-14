namespace Foundation.Domain.Entities;

public sealed class Certification
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<EmployeeCertification> EmployeeCertifications { get; set; } = new List<EmployeeCertification>();
}
