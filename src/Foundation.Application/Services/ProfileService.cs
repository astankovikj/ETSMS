using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Domain.Entities;

namespace Foundation.Application.Services;

public sealed class ProfileService : IProfileService
{
    private const int ExpiryWarningDays = 30;

    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IEmployeeRepository employeeRepository, IProfileRepository profileRepository)
    {
        _employeeRepository = employeeRepository;
        _profileRepository = profileRepository;
    }

    public async Task<ProfileDto?> GetProfileAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        var skillsTask = _profileRepository.GetTechnicalSkillsAsync(employeeId, cancellationToken);
        var domainsTask = _profileRepository.GetDomainExpertiseAsync(employeeId, cancellationToken);
        var certsTask = _profileRepository.GetCertificationsAsync(employeeId, cancellationToken);

        await Task.WhenAll(skillsTask, domainsTask, certsTask);

        var skillGroups = (await skillsTask)
            .GroupBy(s => s.Category.Name)
            .OrderBy(g => g.Key)
            .Select(g => new SkillCategoryGroupDto
            {
                Category = g.Key,
                Skills = g
                    .OrderBy(s => s.Name)
                    .Select(s => new TechnicalSkillDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Proficiency = s.Proficiency.ToString(),
                        LastUpdated = s.LastUpdated
                    })
                    .ToList()
            })
            .ToList();

        var domains = (await domainsTask)
            .OrderBy(d => d.Name)
            .Select(d => new DomainExpertiseDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToList();

        var today = DateTime.UtcNow.Date;
        var certs = (await certsTask)
            .OrderBy(c => c.ExpirationDate)
            .Select(c => new CertificationDto
            {
                Id = c.Id,
                Name = c.Name,
                ExpirationDate = c.ExpirationDate,
                ExpiryStatus = ResolveExpiryStatus(c.ExpirationDate.Date, today)
            })
            .ToList();

        return new ProfileDto
        {
            Employee = new EmployeeDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Role = employee.Role
            },
            TechnicalSkills = skillGroups,
            DomainExpertise = domains,
            Certifications = certs
        };
    }

    private static string ResolveExpiryStatus(DateTime expiryDate, DateTime today)
    {
        if (expiryDate < today)
        {
            return "expired";
        }

        if ((expiryDate - today).TotalDays <= ExpiryWarningDays)
        {
            return "soon";
        }

        return "ok";
    }
}
