using System;
using System.Collections.Generic;

namespace Foundation.Application.DTOs;

public sealed class SkillAssessmentSnapshotDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public DateTime Timestamp { get; init; }
    public IEnumerable<SkillAssessmentEntryDto> Entries { get; init; } = Array.Empty<SkillAssessmentEntryDto>();
}
