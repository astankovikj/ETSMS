namespace Foundation.Domain.Entities;

public sealed class Skill
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}
