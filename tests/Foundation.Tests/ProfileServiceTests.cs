using FluentAssertions;
using Foundation.Application.Interfaces;
using Foundation.Application.Services;
using Foundation.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Foundation.Tests;

/// <summary>
/// Unit tests for <see cref="ProfileService"/> — covers DTO mapping,
/// certification expiry status logic (FR-4), and skill grouping by category (FR-2).
/// </summary>
public sealed class ProfileServiceTests
{
    private readonly IEmployeeRepository _employeeRepo = Substitute.For<IEmployeeRepository>();
    private readonly IProfileRepository _profileRepo = Substitute.For<IProfileRepository>();
    private readonly ProfileService _sut;

    private static readonly Guid EmployeeId = Guid.NewGuid();

    private static readonly Employee SampleEmployee = new()
    {
        Id = EmployeeId,
        FirstName = "Sarah",
        LastName = "Chen",
        Email = "sarah.chen@corp.com",
        Role = "Employee"
    };

    public ProfileServiceTests()
    {
        _sut = new ProfileService(_employeeRepo, _profileRepo);
    }

    // ── AC-7 / FR-6: returns null when employee not found ───────────────────

    [Fact]
    public async Task GetProfileAsync_ReturnsNull_WhenEmployeeNotFound()
    {
        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns((Employee?)null);

        var result = await _sut.GetProfileAsync(EmployeeId);

        result.Should().BeNull();
    }

    // ── FR-2 / AC-1 / AC-2: employee DTO is mapped correctly ────────────────

    [Fact]
    public async Task GetProfileAsync_MapsEmployeeDto_Correctly()
    {
        SetupDefaultRepositories();

        var result = await _sut.GetProfileAsync(EmployeeId);

        result.Should().NotBeNull();
        result!.Employee.Id.Should().Be(EmployeeId);
        result.Employee.FullName.Should().Be("Sarah Chen");
        result.Employee.Email.Should().Be("sarah.chen@corp.com");
        result.Employee.Role.Should().Be("Employee");
    }

    // ── FR-2 / AC-1: technical skills are grouped by category ───────────────

    [Fact]
    public async Task GetProfileAsync_GroupsTechnicalSkillsByCategory()
    {
        var backendCategory = new SkillCategory { Id = Guid.NewGuid(), Name = "Backend" };
        var frontendCategory = new SkillCategory { Id = Guid.NewGuid(), Name = "Frontend" };

        var skills = new List<TechnicalSkill>
        {
            new() { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "C# / .NET Core",   Proficiency = ProficiencyLevel.Expert,       LastUpdated = new DateTime(2024, 7, 1), Category = backendCategory,  CategoryId = backendCategory.Id },
            new() { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "PostgreSQL",        Proficiency = ProficiencyLevel.Advanced,     LastUpdated = new DateTime(2024, 5, 1), Category = backendCategory,  CategoryId = backendCategory.Id },
            new() { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "React / TypeScript", Proficiency = ProficiencyLevel.Advanced,    LastUpdated = new DateTime(2024, 6, 1), Category = frontendCategory, CategoryId = frontendCategory.Id },
        };

        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns(SampleEmployee);
        _profileRepo.GetTechnicalSkillsAsync(EmployeeId, default).Returns(skills);
        _profileRepo.GetDomainExpertiseAsync(EmployeeId, default).Returns(Enumerable.Empty<DomainExpertise>());
        _profileRepo.GetCertificationsAsync(EmployeeId, default).Returns(Enumerable.Empty<Certification>());

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.TechnicalSkills.Should().HaveCount(2);

        var backendGroup = result.TechnicalSkills.First(g => g.Category == "Backend");
        backendGroup.Skills.Should().HaveCount(2);
        backendGroup.Skills.Select(s => s.Name).Should().Contain("C# / .NET Core").And.Contain("PostgreSQL");

        var frontendGroup = result.TechnicalSkills.First(g => g.Category == "Frontend");
        frontendGroup.Skills.Should().HaveCount(1);
        frontendGroup.Skills[0].Name.Should().Be("React / TypeScript");
    }

    // ── FR-2 / AC-2: proficiency label is the enum name ─────────────────────

    [Theory]
    [InlineData(ProficiencyLevel.Beginner,     "Beginner")]
    [InlineData(ProficiencyLevel.Intermediate, "Intermediate")]
    [InlineData(ProficiencyLevel.Advanced,     "Advanced")]
    [InlineData(ProficiencyLevel.Expert,       "Expert")]
    public async Task GetProfileAsync_MapsProficiencyLabel_Correctly(ProficiencyLevel level, string expectedLabel)
    {
        var category = new SkillCategory { Id = Guid.NewGuid(), Name = "Backend" };
        var skills = new List<TechnicalSkill>
        {
            new() { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "Skill", Proficiency = level, LastUpdated = DateTime.UtcNow, Category = category, CategoryId = category.Id }
        };

        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns(SampleEmployee);
        _profileRepo.GetTechnicalSkillsAsync(EmployeeId, default).Returns(skills);
        _profileRepo.GetDomainExpertiseAsync(EmployeeId, default).Returns(Enumerable.Empty<DomainExpertise>());
        _profileRepo.GetCertificationsAsync(EmployeeId, default).Returns(Enumerable.Empty<Certification>());

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.TechnicalSkills[0].Skills[0].Proficiency.Should().Be(expectedLabel);
    }

    // ── FR-3 / AC-3: domain expertise is mapped correctly ───────────────────

    [Fact]
    public async Task GetProfileAsync_MapsDomainExpertise_Correctly()
    {
        var domains = new List<DomainExpertise>
        {
            new() { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "Financial Services" },
            new() { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "DevSecOps" }
        };

        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns(SampleEmployee);
        _profileRepo.GetTechnicalSkillsAsync(EmployeeId, default).Returns(Enumerable.Empty<TechnicalSkill>());
        _profileRepo.GetDomainExpertiseAsync(EmployeeId, default).Returns(domains);
        _profileRepo.GetCertificationsAsync(EmployeeId, default).Returns(Enumerable.Empty<Certification>());

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.DomainExpertise.Should().HaveCount(2);
        result.DomainExpertise.Select(d => d.Name).Should().Contain("Financial Services").And.Contain("DevSecOps");
    }

    // ── FR-4 / AC-4: certification expiry status ─────────────────────────────

    [Fact]
    public async Task GetProfileAsync_SetsExpiryStatus_Ok_WhenMoreThan30DaysAway()
    {
        var futureDate = DateTime.UtcNow.Date.AddDays(31);
        SetupCertifications(new Certification { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "AZ-900", ExpirationDate = futureDate });

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.Certifications[0].ExpiryStatus.Should().Be("ok");
    }

    [Fact]
    public async Task GetProfileAsync_SetsExpiryStatus_Soon_WhenExpiringWithin30Days()
    {
        var soonDate = DateTime.UtcNow.Date.AddDays(15);
        SetupCertifications(new Certification { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "AZ-900", ExpirationDate = soonDate });

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.Certifications[0].ExpiryStatus.Should().Be("soon");
    }

    [Fact]
    public async Task GetProfileAsync_SetsExpiryStatus_Soon_WhenExpiringExactlyIn30Days()
    {
        var boundaryDate = DateTime.UtcNow.Date.AddDays(30);
        SetupCertifications(new Certification { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "AZ-900", ExpirationDate = boundaryDate });

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.Certifications[0].ExpiryStatus.Should().Be("soon");
    }

    [Fact]
    public async Task GetProfileAsync_SetsExpiryStatus_Expired_WhenExpiryDateIsInThePast()
    {
        var pastDate = DateTime.UtcNow.Date.AddDays(-1);
        SetupCertifications(new Certification { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "AZ-900", ExpirationDate = pastDate });

        var result = await _sut.GetProfileAsync(EmployeeId);

        result!.Certifications[0].ExpiryStatus.Should().Be("expired");
    }

    [Fact]
    public async Task GetProfileAsync_SetsExpiryStatus_Expired_WhenExpiryDateIsToday()
    {
        // A cert that expired today (date < today is false; date == today; date - today == 0 days => "soon" boundary)
        // Per FR-4 spec: "expired" means date < today. Today (== today) falls into "soon" (0 days <= 30).
        var todayDate = DateTime.UtcNow.Date;
        SetupCertifications(new Certification { Id = Guid.NewGuid(), EmployeeId = EmployeeId, Name = "AZ-900", ExpirationDate = todayDate });

        var result = await _sut.GetProfileAsync(EmployeeId);

        // Expiry date equals today: not yet past, but 0 days remaining — treated as "soon"
        result!.Certifications[0].ExpiryStatus.Should().Be("soon");
    }

    // ── FR-5 / AC-5: empty sections return empty collections ─────────────────

    [Fact]
    public async Task GetProfileAsync_ReturnsEmptyCollections_WhenNoData()
    {
        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns(SampleEmployee);
        _profileRepo.GetTechnicalSkillsAsync(EmployeeId, default).Returns(Enumerable.Empty<TechnicalSkill>());
        _profileRepo.GetDomainExpertiseAsync(EmployeeId, default).Returns(Enumerable.Empty<DomainExpertise>());
        _profileRepo.GetCertificationsAsync(EmployeeId, default).Returns(Enumerable.Empty<Certification>());

        var result = await _sut.GetProfileAsync(EmployeeId);

        result.Should().NotBeNull();
        result!.TechnicalSkills.Should().BeEmpty();
        result.DomainExpertise.Should().BeEmpty();
        result.Certifications.Should().BeEmpty();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void SetupDefaultRepositories()
    {
        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns(SampleEmployee);
        _profileRepo.GetTechnicalSkillsAsync(EmployeeId, default).Returns(Enumerable.Empty<TechnicalSkill>());
        _profileRepo.GetDomainExpertiseAsync(EmployeeId, default).Returns(Enumerable.Empty<DomainExpertise>());
        _profileRepo.GetCertificationsAsync(EmployeeId, default).Returns(Enumerable.Empty<Certification>());
    }

    private void SetupCertifications(params Certification[] certifications)
    {
        _employeeRepo.GetByIdAsync(EmployeeId, default).Returns(SampleEmployee);
        _profileRepo.GetTechnicalSkillsAsync(EmployeeId, default).Returns(Enumerable.Empty<TechnicalSkill>());
        _profileRepo.GetDomainExpertiseAsync(EmployeeId, default).Returns(Enumerable.Empty<DomainExpertise>());
        _profileRepo.GetCertificationsAsync(EmployeeId, default).Returns(certifications.AsEnumerable());
    }
}
