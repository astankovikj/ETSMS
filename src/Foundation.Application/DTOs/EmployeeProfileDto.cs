namespace Foundation.Application.DTOs;

public sealed class EmployeeProfileDto
{
    public Guid EmployeeId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public IReadOnlyList<TechnicalSkillDto> TechnicalSkills { get; init; } = [];
    public IReadOnlyList<DomainExpertiseDto> DomainExpertise { get; init; } = [];
    public IReadOnlyList<CertificationDto> Certifications { get; init; } = [];
}
