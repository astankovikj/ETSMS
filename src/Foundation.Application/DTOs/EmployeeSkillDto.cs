namespace Foundation.Application.DTOs;

public sealed class EmployeeSkillDto
{
    public Guid EmployeeId { get; init; }
    public int SkillId { get; init; }
    public string SkillName { get; init; } = string.Empty;
    public string? Category { get; init; }
    public int ProficiencyLevel { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
