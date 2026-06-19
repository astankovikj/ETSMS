using System;
using System.Collections.Generic;

namespace Foundation.Application.DTOs;

public sealed class CreateSkillAssessmentRequest
{
    public IEnumerable<CreateSkillAssessmentEntry> Entries { get; init; } = Array.Empty<CreateSkillAssessmentEntry>();
}
