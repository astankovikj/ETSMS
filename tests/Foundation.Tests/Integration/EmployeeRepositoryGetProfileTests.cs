using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Foundation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Tests.Integration;

/// <summary>
/// Integration tests for EmployeeRepository.GetProfileAsync using an in-memory database.
/// </summary>
public class EmployeeRepositoryGetProfileTests : IDisposable
{
    private readonly FoundationDbContext _context;
    private readonly EmployeeRepository _sut;

    public EmployeeRepositoryGetProfileTests()
    {
        var options = new DbContextOptionsBuilder<FoundationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FoundationDbContext(options);
        _sut = new EmployeeRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetProfileAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        // Arrange
        var employee = CreateEmployee("John", "Smith", "j.smith@corp.com");
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetProfileAsync(employee.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
    }

    [Fact]
    public async Task GetProfileAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _sut.GetProfileAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsAllProfileFields()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
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
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetProfileAsync(employee.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Engineering", result.Department);
        Assert.Equal("Software Engineer", result.JobTitle);
        Assert.Equal("+1-555-000-0001", result.PhoneNumber);
        Assert.Equal("https://cdn.corp.com/pics/jane.jpg", result.ProfilePictureUrl);
        Assert.Equal("Passionate engineer.", result.Bio);
        Assert.Equal("New York, USA", result.Location);
    }

    [Fact]
    public async Task GetProfileAsync_WithMultipleEmployees_ReturnsOnlyRequestedEmployee()
    {
        // Arrange
        var employee1 = CreateEmployee("Alice", "Alpha", "a.alpha@corp.com");
        var employee2 = CreateEmployee("Bob", "Beta", "b.beta@corp.com");
        _context.Employees.AddRange(employee1, employee2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetProfileAsync(employee1.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee1.Id, result.Id);
        Assert.Equal("a.alpha@corp.com", result.Email);
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsCoreEmployeeFields()
    {
        // Arrange
        var employee = CreateEmployee("Carol", "Chen", "c.chen@corp.com", role: "Leadership");
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetProfileAsync(employee.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
        Assert.Equal("Carol", result.FirstName);
        Assert.Equal("Chen", result.LastName);
        Assert.Equal("c.chen@corp.com", result.Email);
        Assert.Equal("Leadership", result.Role);
    }

    private static Employee CreateEmployee(string firstName, string lastName, string email, string role = "Employee")
    {
        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role
        };
    }
}
