using Foundation.Application.DTOs;
using Foundation.Domain.Entities;

namespace Foundation.Tests.Unit;

/// <summary>
/// Tests for the EmployeeProfileDto DTO structure — verifies all profile fields exist and are mapped correctly.
/// </summary>
public class EmployeeProfileDtoTests
{
    [Fact]
    public void EmployeeProfileDto_HasAllExpectedProperties()
    {
        var dto = new EmployeeProfileDto
        {
            Id = Guid.NewGuid(),
            FullName = "Jane Doe",
            Email = "jane.doe@corp.com",
            Role = "Employee",
            Department = "Engineering",
            JobTitle = "Software Engineer",
            PhoneNumber = "+1-555-000-0001",
            ProfilePictureUrl = "https://cdn.corp.com/pics/jane.jpg",
            Bio = "Passionate engineer.",
            Location = "New York, USA"
        };

        Assert.Equal("Jane Doe", dto.FullName);
        Assert.Equal("jane.doe@corp.com", dto.Email);
        Assert.Equal("Employee", dto.Role);
        Assert.Equal("Engineering", dto.Department);
        Assert.Equal("Software Engineer", dto.JobTitle);
        Assert.Equal("+1-555-000-0001", dto.PhoneNumber);
        Assert.Equal("https://cdn.corp.com/pics/jane.jpg", dto.ProfilePictureUrl);
        Assert.Equal("Passionate engineer.", dto.Bio);
        Assert.Equal("New York, USA", dto.Location);
    }

    [Fact]
    public void EmployeeProfileDto_DefaultsAreEmptyStrings()
    {
        var dto = new EmployeeProfileDto();

        Assert.Equal(string.Empty, dto.FullName);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Role);
        Assert.Equal(string.Empty, dto.Department);
        Assert.Equal(string.Empty, dto.JobTitle);
        Assert.Equal(string.Empty, dto.PhoneNumber);
        Assert.Equal(string.Empty, dto.ProfilePictureUrl);
        Assert.Equal(string.Empty, dto.Bio);
        Assert.Equal(string.Empty, dto.Location);
        Assert.Equal(Guid.Empty, dto.Id);
    }
}
