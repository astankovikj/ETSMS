using System;

namespace Foundation.Domain.Entities;

public sealed class SkillAssessmentEntry
{
    public Guid Id { get; set; }
    public Guid SnapshotId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int Proficiency { get; set; }
    public string ProficiencyLabel { get; set; } = string.Empty;
    public bool IsNoExperience { get; set; }
    public string? Notes { get; set; }

    public SkillAssessmentSnapshot? Snapshot { get; set; }
}
