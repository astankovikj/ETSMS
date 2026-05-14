namespace Foundation.Domain.Entities;

public enum ProficiencyLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}

public sealed class TechnicalSkill
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProficiencyLevel Proficiency { get; set; }
    public DateTime LastUpdated { get; set; }

    public Employee Employee { get; set; } = null!;
    public SkillCategory Category { get; set; } = null!;
}
