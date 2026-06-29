using Foundation.Application.DTOs;
using Foundation.Application.Interfaces;
using Foundation.Application.Services;
using Foundation.Domain.Entities;
using NSubstitute;

namespace Foundation.Tests.Unit;

/// <summary>
/// Unit tests for EmployeeService.GetProfileAsync — verifies correct mapping and null-return behaviour.
/// </summary>
public class EmployeeServiceGetProfileTests
{
    private readonly IEmployeeRepository _repository;
    private readonly EmployeeService _sut;

    public EmployeeServiceGetProfileTests()
    {
        _repository = Substitute.For<IEmployeeRepository>();
        _sut = new EmployeeService(_repository);
    }

    [Fact]
    public async Task GetProfileAsync_WhenEmployeeExists_ReturnsProfileDtoWithAllFields()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employee = new Employee
        {
            Id = id,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@corp.com",
            Role = "Employee",
            Department = "Engineering",
            JobTitle = "Software Engineer",
            PhoneNumber = "+1-555-000-0001",
            ProfilePictureUrl = "https://cdn.corp.com/pics/jane.jpg",
            Bio = "Passionate engineer.",
            Location = "New York, USA"
        };

        _repository.GetProfileAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Employee?>(employee));

        // Act
        var result = await _sut.GetProfileAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Jane Doe", result.FullName);
        Assert.Equal("jane.doe@corp.com", result.Email);
        Assert.Equal("Employee", result.Role);
        Assert.Equal("Engineering", result.Department);
        Assert.Equal("Software Engineer", result.JobTitle);
        Assert.Equal("+1-555-000-0001", result.PhoneNumber);
        Assert.Equal("https://cdn.corp.com/pics/jane.jpg", result.ProfilePictureUrl);
        Assert.Equal("Passionate engineer.", result.Bio);
        Assert.Equal("New York, USA", result.Location);
    }

    [Fact]
    public async Task GetProfileAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetProfileAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Employee?>(null));

        // Act
        var result = await _sut.GetProfileAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProfileAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetProfileAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Employee?>(null));

        // Act
        await _sut.GetProfileAsync(id, CancellationToken.None);

        // Assert — repository was called exactly once with the correct ID
        await _repository.Received(1).GetProfileAsync(id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProfileAsync_WhenEmployeeExists_ReturnsDtoOfCorrectType()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employee = new Employee { Id = id, FirstName = "Test", LastName = "User", Email = "t@t.com", Role = "Employee" };
        _repository.GetProfileAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Employee?>(employee));

        // Act
        var result = await _sut.GetProfileAsync(id, CancellationToken.None);

        // Assert
        Assert.IsType<EmployeeProfileDto>(result);
    }

    [Fact]
    public async Task GetProfileAsync_FullName_IsComputedFromFirstAndLastName()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employee = new Employee { Id = id, FirstName = "Alice", LastName = "Smith", Email = "a@s.com", Role = "Employee" };
        _repository.GetProfileAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Employee?>(employee));

        // Act
        var result = await _sut.GetProfileAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice Smith", result.FullName);
    }

    [Fact]
    public async Task GetProfileAsync_WithEmptyProfileFields_ReturnsDtoWithEmptyStrings()
    {
        // Arrange — employee with no profile info set
        var id = Guid.NewGuid();
        var employee = new Employee
        {
            Id = id,
            FirstName = "Bob",
            LastName = "Jones",
            Email = "b@j.com",
            Role = "Employee"
            // Profile fields are all default empty strings
        };

        _repository.GetProfileAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Employee?>(employee));

        // Act
        var result = await _sut.GetProfileAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Department);
        Assert.Equal(string.Empty, result.JobTitle);
        Assert.Equal(string.Empty, result.PhoneNumber);
        Assert.Equal(string.Empty, result.ProfilePictureUrl);
        Assert.Equal(string.Empty, result.Bio);
        Assert.Equal(string.Empty, result.Location);
    }
}
