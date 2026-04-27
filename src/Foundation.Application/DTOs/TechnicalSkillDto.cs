namespace Foundation.Application.DTOs;

public sealed class TechnicalSkillDto
{
    public Guid Id { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int ProficiencyLevel { get; init; }
    public DateTime LastUpdatedAt { get; init; }
}
