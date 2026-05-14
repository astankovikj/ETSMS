namespace Foundation.Application.DTOs;

public sealed class ProfileDto
{
    public EmployeeDto Employee { get; init; } = null!;
    public IReadOnlyList<SkillCategoryGroupDto> TechnicalSkills { get; init; } = [];
    public IReadOnlyList<DomainExpertiseDto> DomainExpertise { get; init; } = [];
    public IReadOnlyList<CertificationDto> Certifications { get; init; } = [];
}

public sealed class SkillCategoryGroupDto
{
    public string Category { get; init; } = string.Empty;
    public IReadOnlyList<TechnicalSkillDto> Skills { get; init; } = [];
}

public sealed class TechnicalSkillDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Proficiency { get; init; } = string.Empty;
    public DateTime LastUpdated { get; init; }
}

public sealed class DomainExpertiseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class CertificationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime ExpirationDate { get; init; }

    /// <summary>
    /// "ok" | "soon" | "expired" — derived from ExpirationDate relative to today.
    /// "soon" means expiring within 30 days but not yet expired.
    /// </summary>
    public string ExpiryStatus { get; init; } = "ok";
}
