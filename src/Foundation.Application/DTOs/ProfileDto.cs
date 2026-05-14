namespace Foundation.Application.DTOs;

public sealed class ProfileDto
{
    public EmployeeInfoDto Employee { get; init; } = new();
    public IReadOnlyList<TechnicalSkillDto> TechnicalSkills { get; init; } = [];
    public IReadOnlyList<DomainExpertiseDto> DomainExpertise { get; init; } = [];
    public IReadOnlyList<CertificationDto> Certifications { get; init; } = [];
}

public sealed class EmployeeInfoDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
}

public sealed class TechnicalSkillDto
{
    public string SkillId { get; init; } = string.Empty;
    public string SkillName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;

    /// <summary>Proficiency level on a scale of 1–5.</summary>
    public int ProficiencyLevel { get; init; }

    public DateTime LastUpdatedDate { get; init; }
}

public sealed class DomainExpertiseDto
{
    public string DomainId { get; init; } = string.Empty;
    public string DomainName { get; init; } = string.Empty;
}

public sealed class CertificationDto
{
    public string CertificationId { get; init; } = string.Empty;
    public string CertificationName { get; init; } = string.Empty;
    public DateTime ExpirationDate { get; init; }
}
