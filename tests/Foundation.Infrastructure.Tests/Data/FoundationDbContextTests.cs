using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Tests.Data;

public class FoundationDbContextTests
{
    private static FoundationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<FoundationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new FoundationDbContext(options);
    }

    [Fact]
    public void DbContext_CanBeInstantiated()
    {
        using var ctx = CreateContext(nameof(DbContext_CanBeInstantiated));
        ctx.Should().NotBeNull();
    }

    [Fact]
    public async Task Employees_DbSet_IsAccessible()
    {
        await using var ctx = CreateContext(nameof(Employees_DbSet_IsAccessible));
        var all = await ctx.Employees.ToListAsync();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task Employees_CanAddAndRetrieve()
    {
        await using var ctx = CreateContext(nameof(Employees_CanAddAndRetrieve));
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Role = "Employee"
        };

        await ctx.Employees.AddAsync(employee);
        await ctx.SaveChangesAsync();

        var saved = await ctx.Employees.FindAsync(employee.Id);
        saved.Should().NotBeNull();
        saved!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void OnModelCreating_ConfiguresEmployeePrimaryKey()
    {
        using var ctx = CreateContext(nameof(OnModelCreating_ConfiguresEmployeePrimaryKey));
        var entityType = ctx.Model.FindEntityType(typeof(Employee));
        entityType.Should().NotBeNull();

        var pk = entityType!.FindPrimaryKey();
        pk.Should().NotBeNull();
        pk!.Properties.Should().ContainSingle(p => p.Name == "Id");
    }

    [Fact]
    public void OnModelCreating_SetsEmailMaxLength250()
    {
        using var ctx = CreateContext(nameof(OnModelCreating_SetsEmailMaxLength250));
        var entityType = ctx.Model.FindEntityType(typeof(Employee));
        var emailProp = entityType!.FindProperty("Email");
        emailProp.Should().NotBeNull();
        emailProp!.GetMaxLength().Should().Be(250);
    }

    [Fact]
    public void OnModelCreating_SetsRoleMaxLength100()
    {
        using var ctx = CreateContext(nameof(OnModelCreating_SetsRoleMaxLength100));
        var entityType = ctx.Model.FindEntityType(typeof(Employee));
        var roleProp = entityType!.FindProperty("Role");
        roleProp.Should().NotBeNull();
        roleProp!.GetMaxLength().Should().Be(100);
    }

    [Fact]
    public void OnModelCreating_EmailIsRequired()
    {
        using var ctx = CreateContext(nameof(OnModelCreating_EmailIsRequired));
        var entityType = ctx.Model.FindEntityType(typeof(Employee));
        var emailProp = entityType!.FindProperty("Email");
        emailProp!.IsNullable.Should().BeFalse("Email is configured as required");
    }
}
