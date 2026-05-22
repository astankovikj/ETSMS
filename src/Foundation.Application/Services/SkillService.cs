using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;

namespace Foundation.Application.Services;

public sealed class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;
    private readonly IEmployeeSkillRepository _employeeSkillRepository;

    public SkillService(ISkillRepository skillRepository, IEmployeeSkillRepository employeeSkillRepository)
    {
        _skillRepository = skillRepository;
        _employeeSkillRepository = employeeSkillRepository;
    }

    public async Task<IEnumerable<SkillDto>> SearchSkillsAsync(string query, CancellationToken cancellationToken = default)
    {
        var skills = await _skillRepository.SearchAsync(query, cancellationToken);

        return skills.Select(s => new SkillDto
        {
            SkillId = s.SkillId,
            SkillName = s.SkillName,
            Category = s.Category
        });
    }

    public async Task<AddEmployeeSkillResult> AddEmployeeSkillAsync(
        Guid employeeId,
        AddEmployeeSkillDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.ProficiencyLevel < 1 || dto.ProficiencyLevel > 5)
        {
            return AddEmployeeSkillResult.Invalid("Proficiency level must be between 1 and 5.");
        }

        var skill = await _skillRepository.GetByIdAsync(dto.SkillId, cancellationToken);
        if (skill is null)
        {
            return AddEmployeeSkillResult.NotFound($"Skill with ID {dto.SkillId} was not found.");
        }

        var alreadyExists = await _employeeSkillRepository.ExistsAsync(employeeId, dto.SkillId, cancellationToken);
        if (alreadyExists)
        {
            return AddEmployeeSkillResult.Conflict(
                $"You have already added {skill.SkillName} to your profile.");
        }

        var now = DateTime.UtcNow;
        var employeeSkill = new EmployeeSkill
        {
            EmployeeId = employeeId,
            SkillId = dto.SkillId,
            ProficiencyLevel = dto.ProficiencyLevel,
            Notes = dto.Notes,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _employeeSkillRepository.AddAsync(employeeSkill, cancellationToken);

        return AddEmployeeSkillResult.Success(new EmployeeSkillDto
        {
            EmployeeId = employeeId,
            SkillId = skill.SkillId,
            SkillName = skill.SkillName,
            Category = skill.Category,
            ProficiencyLevel = dto.ProficiencyLevel,
            Notes = dto.Notes,
            CreatedAt = employeeSkill.CreatedAt,
            UpdatedAt = employeeSkill.UpdatedAt
        });
    }
}
