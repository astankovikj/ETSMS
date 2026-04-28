using Foundation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Foundation.Infrastructure.Data;

public sealed class FoundationDbContext : DbContext
{
    public FoundationDbContext(DbContextOptions<FoundationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Domain> Domains => Set<Domain>();
    public DbSet<EmployeeDomain> EmployeeDomains => Set<EmployeeDomain>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Role).HasMaxLength(100);
        });

        modelBuilder.Entity<Domain>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(50);
            builder.Property(d => d.Type).IsRequired().HasMaxLength(50);
            builder.Property(d => d.Description).HasMaxLength(200);
            builder.HasIndex(d => d.Name).IsUnique();
        });

        modelBuilder.Entity<EmployeeDomain>(builder =>
        {
            builder.HasKey(ed => new { ed.EmployeeId, ed.DomainId });

            builder.HasOne(ed => ed.Employee)
                .WithMany(e => e.EmployeeDomains)
                .HasForeignKey(ed => ed.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ed => ed.Domain)
                .WithMany(d => d.EmployeeDomains)
                .HasForeignKey(ed => ed.DomainId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
