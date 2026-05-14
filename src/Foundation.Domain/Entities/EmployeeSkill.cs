namespace Foundation.Domain.Entities;

public sealed class EmployeeSkill
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    /// <summary>Proficiency level on a scale of 1 (beginner) to 5 (expert).</summary>
    public int ProficiencyLevel { get; set; }

    public DateTime LastUpdatedDate { get; set; }
}
