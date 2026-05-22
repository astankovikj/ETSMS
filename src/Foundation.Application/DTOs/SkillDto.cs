namespace Foundation.Application.DTOs;

public sealed class SkillDto
{
    public int SkillId { get; init; }
    public string SkillName { get; init; } = string.Empty;
    public string? Category { get; init; }
}
