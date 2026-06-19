using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Application.Services;
using Foundation.Domain.Entities;
using Moq;
using Xunit;

namespace Foundation.Application.Tests;

public class SkillAssessmentServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidEntries_ReturnsSnapshotDtoAndPersists()
    {
        var repository = new Mock<ISkillAssessmentRepository>();
        SkillAssessmentSnapshot? persistedSnapshot = null;

        repository
            .Setup(r => r.AddSnapshotAsync(It.IsAny<SkillAssessmentSnapshot>(), It.IsAny<CancellationToken>()))
            .Callback<SkillAssessmentSnapshot, CancellationToken>((snapshot, _) => persistedSnapshot = snapshot)
            .Returns(Task.CompletedTask);

        var service = new SkillAssessmentService(repository.Object);
        var entry = new CreateSkillAssessmentEntry
        {
            SkillName = "Azure",
            Proficiency = 4,
            ProficiencyLabel = "Advanced",
            IsNoExperience = true,
            Notes = "Best guess"
        };

        var request = new CreateSkillAssessmentRequest
        {
            Entries = new[] { entry }
        };

        var employeeId = Guid.NewGuid();

        var result = await service.CreateAsync(employeeId, request);

        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Single(result.Entries);
        var dtoEntry = Assert.Single(result.Entries);
        Assert.Equal("Azure", dtoEntry.SkillName);
        Assert.Equal(4, dtoEntry.Proficiency);
        Assert.Equal("Advanced", dtoEntry.ProficiencyLabel);
        Assert.True(dtoEntry.IsNoExperience);
        Assert.Equal("Best guess", dtoEntry.Notes);

        Assert.NotNull(persistedSnapshot);
        Assert.Equal(1, persistedSnapshot!.Entries.Count);
        Assert.Equal("Advanced", persistedSnapshot.Entries.Single().ProficiencyLabel);
        repository.Verify(r => r.AddSnapshotAsync(It.IsAny<SkillAssessmentSnapshot>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNoEntries_ThrowsArgumentException()
    {
        var repository = new Mock<ISkillAssessmentRepository>();
        var service = new SkillAssessmentService(repository.Object);

        var request = new CreateSkillAssessmentRequest
        {
            Entries = Array.Empty<CreateSkillAssessmentEntry>()
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(Guid.NewGuid(), request));
        Assert.Equal("At least one skill entry must be provided.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidProficiency_ThrowsArgumentException()
    {
        var repository = new Mock<ISkillAssessmentRepository>();
        var service = new SkillAssessmentService(repository.Object);

        var request = new CreateSkillAssessmentRequest
        {
            Entries = new[]
            {
                new CreateSkillAssessmentEntry
                {
                    SkillName = "Cloud",
                    Proficiency = 6
                }
            }
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(Guid.NewGuid(), request));
        Assert.Contains("must be between 1 and 5", exception.Message);
    }

    [Fact]
    public async Task GetLatestSnapshotAsync_WhenNoSnapshot_ReturnsEmptySnapshotDto()
    {
        var repository = new Mock<ISkillAssessmentRepository>();
        var employeeId = Guid.NewGuid();

        repository
            .Setup(r => r.GetLatestSnapshotAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SkillAssessmentSnapshot?)null);

        var service = new SkillAssessmentService(repository.Object);

        var result = await service.GetLatestSnapshotAsync(employeeId);

        Assert.Equal(Guid.Empty, result.Id);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Equal(DateTime.MinValue, result.Timestamp);
        Assert.Empty(result.Entries);
    }

    [Fact]
    public async Task ListLatestByEmployeesAsync_ReturnsSnapshotDtos()
    {
        var repository = new Mock<ISkillAssessmentRepository>();
        var snapshot = new SkillAssessmentSnapshot
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            Entries = new List<SkillAssessmentEntry>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SkillName = "Terraform",
                    Proficiency = 3,
                    ProficiencyLabel = "Intermediate",
                    IsNoExperience = false
                }
            }
        };

        repository
            .Setup(r => r.ListLatestSnapshotsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { snapshot });

        var service = new SkillAssessmentService(repository.Object);

        var result = await service.ListLatestByEmployeesAsync();
        var dto = Assert.Single(result);

        Assert.Equal(snapshot.Id, dto.Id);
        Assert.Equal(snapshot.EmployeeId, dto.EmployeeId);
        Assert.Single(dto.Entries);
        var entryDto = Assert.Single(dto.Entries);
        Assert.Equal("Terraform", entryDto.SkillName);
        Assert.Equal(3, entryDto.Proficiency);
        Assert.Equal("Intermediate", entryDto.ProficiencyLabel);
        Assert.False(entryDto.IsNoExperience);
    }
}
