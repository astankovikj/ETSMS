namespace Foundation.Domain.Entities;

public sealed class TechnicalSkill
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// <summary>Proficiency level on a 1–5 scale.</summary>
    public int ProficiencyLevel { get; set; }

    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Employee? Employee { get; set; }
}
