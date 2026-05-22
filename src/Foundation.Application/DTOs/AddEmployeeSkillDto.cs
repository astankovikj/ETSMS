namespace Foundation.Application.DTOs;

public sealed class AddEmployeeSkillDto
{
    public int SkillId { get; init; }
    public int ProficiencyLevel { get; init; }
    public string? Notes { get; init; }
}
