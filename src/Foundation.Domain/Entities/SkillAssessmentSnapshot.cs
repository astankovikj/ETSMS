using System;
using System.Collections.Generic;

namespace Foundation.Domain.Entities;

public sealed class SkillAssessmentSnapshot
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime Timestamp { get; set; }
    public List<SkillAssessmentEntry> Entries { get; set; } = new();

    public Employee? Employee { get; set; }
}
