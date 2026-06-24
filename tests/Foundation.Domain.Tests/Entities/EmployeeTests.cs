using Foundation.Domain.Entities;
using FluentAssertions;

namespace Foundation.Domain.Tests.Entities;

public class EmployeeTests
{
    // ── FullName computed property ───────────────────────────────────────────

    [Fact]
    public void FullName_ReturnsConcatenatedFirstAndLastName()
    {
        var employee = new Employee { FirstName = "Jane", LastName = "Doe" };

        employee.FullName.Should().Be("Jane Doe");
    }

    [Fact]
    public void FullName_WhenFirstNameIsEmpty_StartsWithSpace()
    {
        // Edge-case: empty first name produces " LastName" because the
        // implementation is "{FirstName} {LastName}" with a literal space.
        var employee = new Employee { FirstName = string.Empty, LastName = "Smith" };

        employee.FullName.Should().Be(" Smith");
    }

    [Fact]
    public void FullName_WhenLastNameIsEmpty_EndsWithSpace()
    {
        var employee = new Employee { FirstName = "John", LastName = string.Empty };

        employee.FullName.Should().Be("John ");
    }

    [Fact]
    public void FullName_WhenBothNamesEmpty_IsASingleSpace()
    {
        var employee = new Employee { FirstName = string.Empty, LastName = string.Empty };

        employee.FullName.Should().Be(" ");
    }

    // ── Default property values ─────────────────────────────────────────────

    [Fact]
    public void NewEmployee_DefaultStringPropertiesAreEmptyString()
    {
        var employee = new Employee();

        employee.FirstName.Should().BeEmpty();
        employee.LastName.Should().BeEmpty();
        employee.Email.Should().BeEmpty();
        employee.Role.Should().BeEmpty();
    }

    [Fact]
    public void NewEmployee_IdDefaultsToEmptyGuid()
    {
        // Guid is a value type; its default is Guid.Empty.
        var employee = new Employee();

        employee.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void NewEmployee_CreatedAtIsApproximatelyUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var employee = new Employee();
        var after = DateTime.UtcNow.AddSeconds(1);

        employee.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    // ── Property round-trip ──────────────────────────────────────────────────

    [Fact]
    public void Employee_PropertiesRoundTrip()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var employee = new Employee
        {
            Id = id,
            FirstName = "Alice",
            LastName = "Wonderland",
            Email = "alice@example.com",
            Role = "Admin",
            CreatedAt = createdAt
        };

        employee.Id.Should().Be(id);
        employee.FirstName.Should().Be("Alice");
        employee.LastName.Should().Be("Wonderland");
        employee.Email.Should().Be("alice@example.com");
        employee.Role.Should().Be("Admin");
        employee.CreatedAt.Should().Be(createdAt);
    }

    // ── Mutability ───────────────────────────────────────────────────────────

    [Fact]
    public void Employee_PropertiesAreMutable()
    {
        var employee = new Employee { FirstName = "Old", LastName = "Name" };

        employee.FirstName = "New";
        employee.LastName = "Value";

        employee.FullName.Should().Be("New Value");
    }

    // ── Unicode / special characters ─────────────────────────────────────────

    [Fact]
    public void FullName_WithUnicodeCharacters_ReturnsCorrectResult()
    {
        var employee = new Employee { FirstName = "Ólafur", LastName = "Björnsson" };

        employee.FullName.Should().Be("Ólafur Björnsson");
    }
}
