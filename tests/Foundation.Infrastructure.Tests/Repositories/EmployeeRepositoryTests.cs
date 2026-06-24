using Foundation.Domain.Entities;
using Foundation.Infrastructure.Data;
using Foundation.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Tests.Repositories;

/// <summary>
/// Tests the <see cref="EmployeeRepository"/> using the EF Core InMemory provider
/// so no external database is required.
/// </summary>
public class EmployeeRepositoryTests
{
    private static FoundationDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<FoundationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new FoundationDbContext(options);
    }

    // ─── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        await using var ctx = CreateContext(nameof(GetAllAsync_WhenEmpty_ReturnsEmptyList));
        var repo = new EmployeeRepository(ctx);

        var result = await repo.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnAllSeedEmployees()
    {
        await using var ctx = CreateContext(nameof(GetAllAsync_ReturnAllSeedEmployees));
        ctx.Employees.AddRange(
            new Employee { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "A", Email = "a@x.com" },
            new Employee { Id = Guid.NewGuid(), FirstName = "Bob",   LastName = "B", Email = "b@x.com" }
        );
        await ctx.SaveChangesAsync();

        var repo = new EmployeeRepository(ctx);
        var result = await repo.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_IsSortedByLastNameThenFirstName()
    {
        await using var ctx = CreateContext(nameof(GetAllAsync_IsSortedByLastNameThenFirstName));
        ctx.Employees.AddRange(
            new Employee { Id = Guid.NewGuid(), FirstName = "Zara",  LastName = "Smith",  Email = "z@x.com" },
            new Employee { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith",  Email = "a@x.com" },
            new Employee { Id = Guid.NewGuid(), FirstName = "John",  LastName = "Adams",  Email = "j@x.com" }
        );
        await ctx.SaveChangesAsync();

        var repo = new EmployeeRepository(ctx);
        var result = (await repo.GetAllAsync()).ToList();

        result[0].LastName.Should().Be("Adams");
        result[1].FirstName.Should().Be("Alice"); // Smith, Alice before Smith, Zara
        result[2].FirstName.Should().Be("Zara");
    }

    [Fact]
    public async Task GetAllAsync_AcceptsCancellationToken()
    {
        await using var ctx = CreateContext(nameof(GetAllAsync_AcceptsCancellationToken));
        var repo = new EmployeeRepository(ctx);

        using var cts = new CancellationTokenSource();
        var result = await repo.GetAllAsync(cts.Token);

        result.Should().NotBeNull();
    }

    // ─── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsEmployee()
    {
        var id = Guid.NewGuid();
        await using var ctx = CreateContext(nameof(GetByIdAsync_WhenExists_ReturnsEmployee));
        ctx.Employees.Add(new Employee { Id = id, FirstName = "Carol", LastName = "C", Email = "c@x.com" });
        await ctx.SaveChangesAsync();

        var repo = new EmployeeRepository(ctx);
        var result = await repo.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.FirstName.Should().Be("Carol");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        await using var ctx = CreateContext(nameof(GetByIdAsync_WhenNotExists_ReturnsNull));
        var repo = new EmployeeRepository(ctx);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        await using var ctx = CreateContext(nameof(GetByIdAsync_WithEmptyGuid_ReturnsNull));
        ctx.Employees.Add(new Employee { Id = Guid.NewGuid(), FirstName = "Dan", LastName = "D", Email = "d@x.com" });
        await ctx.SaveChangesAsync();

        var repo = new EmployeeRepository(ctx);
        var result = await repo.GetByIdAsync(Guid.Empty);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_AcceptsCancellationToken()
    {
        await using var ctx = CreateContext(nameof(GetByIdAsync_AcceptsCancellationToken));
        var repo = new EmployeeRepository(ctx);

        using var cts = new CancellationTokenSource();
        var result = await repo.GetByIdAsync(Guid.NewGuid(), cts.Token);

        result.Should().BeNull();
    }

    // ─── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_PersistsEmployeeToDatabase()
    {
        var id = Guid.NewGuid();
        await using var ctx = CreateContext(nameof(AddAsync_PersistsEmployeeToDatabase));
        var repo = new EmployeeRepository(ctx);

        await repo.AddAsync(new Employee { Id = id, FirstName = "Eve", LastName = "E", Email = "e@x.com", Role = "Employee" });

        var stored = await ctx.Employees.FindAsync(id);
        stored.Should().NotBeNull();
        stored!.Email.Should().Be("e@x.com");
    }

    [Fact]
    public async Task AddAsync_SavesChangesImmediately()
    {
        var id = Guid.NewGuid();
        // Use two different context instances to verify SaveChanges was called.
        var options = new DbContextOptionsBuilder<FoundationDbContext>()
            .UseInMemoryDatabase(nameof(AddAsync_SavesChangesImmediately))
            .Options;

        await using (var writeCtx = new FoundationDbContext(options))
        {
            var repo = new EmployeeRepository(writeCtx);
            await repo.AddAsync(new Employee { Id = id, FirstName = "Frank", LastName = "F", Email = "f@x.com" });
        }

        await using var readCtx = new FoundationDbContext(options);
        var stored = await readCtx.Employees.FindAsync(id);
        stored.Should().NotBeNull("AddAsync must call SaveChangesAsync so data is durable");
    }

    [Fact]
    public async Task AddAsync_MultipleEmployees_AllPersisted()
    {
        await using var ctx = CreateContext(nameof(AddAsync_MultipleEmployees_AllPersisted));
        var repo = new EmployeeRepository(ctx);
        for (var i = 0; i < 5; i++)
        {
            await repo.AddAsync(new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = $"First{i}",
                LastName = $"Last{i}",
                Email = $"emp{i}@x.com"
            });
        }

        ctx.Employees.Count().Should().Be(5);
    }

    [Fact]
    public async Task AddAsync_AcceptsCancellationToken()
    {
        var id = Guid.NewGuid();
        await using var ctx = CreateContext(nameof(AddAsync_AcceptsCancellationToken));
        var repo = new EmployeeRepository(ctx);

        using var cts = new CancellationTokenSource();
        await repo.AddAsync(new Employee { Id = id, FirstName = "Grace", LastName = "G", Email = "g@x.com" }, cts.Token);

        (await ctx.Employees.FindAsync(id)).Should().NotBeNull();
    }
}
