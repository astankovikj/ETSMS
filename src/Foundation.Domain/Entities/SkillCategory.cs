namespace Foundation.Domain.Entities;

public sealed class SkillCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TechnicalSkill> TechnicalSkills { get; set; } = new List<TechnicalSkill>();
}
