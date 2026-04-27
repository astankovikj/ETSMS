using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;

namespace Foundation.Application.Services;

public sealed class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public ProfileService(IProfileRepository profileRepository, IEmployeeRepository employeeRepository)
    {
        _profileRepository = profileRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeProfileDto?> GetMyProfileAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        var technicalSkills = await _profileRepository.GetTechnicalSkillsAsync(employeeId, cancellationToken);
        var domainExpertise = await _profileRepository.GetDomainExpertiseAsync(employeeId, cancellationToken);
        var certifications = await _profileRepository.GetCertificationsAsync(employeeId, cancellationToken);

        return new EmployeeProfileDto
        {
            EmployeeId = employee.Id,
            FullName = employee.FullName,
            TechnicalSkills = technicalSkills
                .Select(s => new TechnicalSkillDto
                {
                    Id = s.Id,
                    Category = s.Category,
                    Name = s.Name,
                    ProficiencyLevel = s.ProficiencyLevel,
                    LastUpdatedAt = s.LastUpdatedAt
                })
                .ToList()
                .AsReadOnly(),
            DomainExpertise = domainExpertise
                .Select(d => new DomainExpertiseDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    LastUpdatedAt = d.LastUpdatedAt
                })
                .ToList()
                .AsReadOnly(),
            Certifications = certifications
                .Select(c => new CertificationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ExpiresAt = c.ExpiresAt
                })
                .ToList()
                .AsReadOnly()
        };
    }
}
