namespace Foundation.Domain.Entities;

public sealed class EmployeeCertification
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public Guid CertificationId { get; set; }
    public Certification Certification { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }
}
