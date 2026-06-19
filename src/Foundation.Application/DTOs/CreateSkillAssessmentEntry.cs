using System;

namespace Foundation.Application.DTOs;

public sealed class CreateSkillAssessmentEntry
{
    public string SkillName { get; init; } = string.Empty;
    public int Proficiency { get; init; }
    public string ProficiencyLabel { get; init; } = string.Empty;
    public bool IsNoExperience { get; init; }
    public string? Notes { get; init; }
}
