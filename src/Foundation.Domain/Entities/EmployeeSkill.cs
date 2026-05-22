namespace Foundation.Domain.Entities;

public sealed class EmployeeSkill
{
    public Guid EmployeeId { get; set; }
    public int SkillId { get; set; }
    public int ProficiencyLevel { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Employee Employee { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
