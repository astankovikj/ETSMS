using Foundation.Domain.Entities;

namespace Foundation.Tests.Unit;

/// <summary>
/// Tests for the profile fields added to the Employee domain entity (US-275).
/// </summary>
public class EmployeeEntityProfileFieldsTests
{
    [Fact]
    public void Employee_HasDepartmentProperty()
    {
        var employee = new Employee { Department = "Sales" };
        Assert.Equal("Sales", employee.Department);
    }

    [Fact]
    public void Employee_HasJobTitleProperty()
    {
        var employee = new Employee { JobTitle = "Account Executive" };
        Assert.Equal("Account Executive", employee.JobTitle);
    }

    [Fact]
    public void Employee_HasPhoneNumberProperty()
    {
        var employee = new Employee { PhoneNumber = "+1-555-123-4567" };
        Assert.Equal("+1-555-123-4567", employee.PhoneNumber);
    }

    [Fact]
    public void Employee_HasProfilePictureUrlProperty()
    {
        var employee = new Employee { ProfilePictureUrl = "https://cdn.corp.com/pics/user.jpg" };
        Assert.Equal("https://cdn.corp.com/pics/user.jpg", employee.ProfilePictureUrl);
    }

    [Fact]
    public void Employee_HasBioProperty()
    {
        var employee = new Employee { Bio = "A brief bio." };
        Assert.Equal("A brief bio.", employee.Bio);
    }

    [Fact]
    public void Employee_HasLocationProperty()
    {
        var employee = new Employee { Location = "San Francisco, CA" };
        Assert.Equal("San Francisco, CA", employee.Location);
    }

    [Fact]
    public void Employee_ProfileFieldsDefaultToEmptyStrings()
    {
        var employee = new Employee();

        Assert.Equal(string.Empty, employee.Department);
        Assert.Equal(string.Empty, employee.JobTitle);
        Assert.Equal(string.Empty, employee.PhoneNumber);
        Assert.Equal(string.Empty, employee.ProfilePictureUrl);
        Assert.Equal(string.Empty, employee.Bio);
        Assert.Equal(string.Empty, employee.Location);
    }

    [Fact]
    public void Employee_FullName_CombinesFirstAndLastName()
    {
        var employee = new Employee { FirstName = "Jane", LastName = "Doe" };
        Assert.Equal("Jane Doe", employee.FullName);
    }
}
