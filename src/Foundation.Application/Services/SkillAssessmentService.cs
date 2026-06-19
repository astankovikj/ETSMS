using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;

namespace Foundation.Application.Services;

public sealed class SkillAssessmentService : ISkillAssessmentService
{
    private readonly ISkillAssessmentRepository _repository;

    private static readonly IReadOnlyDictionary<int, string> ProficiencyLabels = new Dictionary<int, string>
    {
        [1] = "Beginner",
        [2] = "Basic",
        [3] = "Intermediate",
        [4] = "Advanced",
        [5] = "Expert"
    };

    public SkillAssessmentService(ISkillAssessmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<SkillAssessmentSnapshotDto> CreateAsync(Guid employeeId, CreateSkillAssessmentRequest request, CancellationToken cancellationToken = default)
    {
        var entries = ValidateEntries(request.Entries);

        var snapshot = new SkillAssessmentSnapshot
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Timestamp = DateTime.UtcNow,
            Entries = entries.Select(e => new SkillAssessmentEntry
            {
                Id = Guid.NewGuid(),
                SkillName = e.SkillName,
                Proficiency = e.Proficiency,
                ProficiencyLabel = ProficiencyLabels[e.Proficiency],
                IsNoExperience = e.IsNoExperience,
                Notes = e.Notes
            }).ToList()
        };

        await _repository.AddSnapshotAsync(snapshot, cancellationToken);

        return ToDto(snapshot);
    }

    public async Task<SkillAssessmentSnapshotDto> GetLatestSnapshotAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var snapshot = await _repository.GetLatestSnapshotAsync(employeeId, cancellationToken);
        if (snapshot is null)
        {
            return new SkillAssessmentSnapshotDto
            {
                Id = Guid.Empty,
                EmployeeId = employeeId,
                Timestamp = DateTime.MinValue,
                Entries = Array.Empty<SkillAssessmentEntryDto>()
            };
        }

        return ToDto(snapshot);
    }

    public async Task<IEnumerable<SkillAssessmentSnapshotDto>> ListLatestByEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var snapshots = await _repository.ListLatestSnapshotsAsync(cancellationToken);
        return snapshots.Select(ToDto);
    }

    private static IEnumerable<CreateSkillAssessmentEntry> ValidateEntries(IEnumerable<CreateSkillAssessmentEntry> entries)
    {
        var list = entries.ToList();
        if (!list.Any())
        {
            throw new ArgumentException("At least one skill entry must be provided.");
        }

        foreach (var entry in list)
        {
            if (entry.Proficiency < 1 || entry.Proficiency > 5)
            {
                throw new ArgumentException($"Proficiency for {entry.SkillName} must be between 1 and 5.");
            }

            if (!ProficiencyLabels.ContainsKey(entry.Proficiency))
            {
                throw new ArgumentException($"Proficiency label for {entry.SkillName} is invalid.");
            }
        }

        return list;
    }

    private static SkillAssessmentSnapshotDto ToDto(SkillAssessmentSnapshot snapshot)
    {
        return new SkillAssessmentSnapshotDto
        {
            Id = snapshot.Id,
            EmployeeId = snapshot.EmployeeId,
            Timestamp = snapshot.Timestamp,
            Entries = snapshot.Entries.Select(entry => new SkillAssessmentEntryDto
            {
                Id = entry.Id,
                SkillName = entry.SkillName,
                Proficiency = entry.Proficiency,
                ProficiencyLabel = entry.ProficiencyLabel,
                IsNoExperience = entry.IsNoExperience,
                Notes = entry.Notes
            }).ToList()
        };
    }
}
