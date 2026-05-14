using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;

namespace Foundation.Application.Services;

public sealed class ProfileService : IProfileService
{
    private readonly IProfileRepository _repository;

    public ProfileService(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfileDto?> GetMyProfileAsync(string email, CancellationToken cancellationToken = default)
    {
        var employee = await _repository.GetByEmailAsync(email, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        var technicalSkills = employee.EmployeeSkills
            .Select(es => new TechnicalSkillDto
            {
                SkillId          = es.SkillId.ToString(),
                SkillName        = es.Skill.Name,
                Category         = es.Skill.Category.Name,
                ProficiencyLevel = es.ProficiencyLevel,
                LastUpdatedDate  = es.LastUpdatedDate
            })
            .OrderBy(s => s.Category)
            .ThenBy(s => s.SkillName)
            .ToList();

        var domainExpertise = employee.EmployeeDomains
            .Select(ed => new DomainExpertiseDto
            {
                DomainId   = ed.DomainId.ToString(),
                DomainName = ed.Domain.Name
            })
            .OrderBy(d => d.DomainName)
            .ToList();

        var certifications = employee.EmployeeCertifications
            .Select(ec => new CertificationDto
            {
                CertificationId   = ec.CertificationId.ToString(),
                CertificationName = ec.Certification.Name,
                ExpirationDate    = ec.ExpirationDate
            })
            .OrderBy(c => c.CertificationName)
            .ToList();

        return new ProfileDto
        {
            Employee = new EmployeeInfoDto
            {
                Id         = employee.Id.ToString(),
                Name       = employee.FullName,
                Role       = employee.Role,
                Department = employee.Department
            },
            TechnicalSkills = technicalSkills,
            DomainExpertise = domainExpertise,
            Certifications  = certifications
        };
    }
}
