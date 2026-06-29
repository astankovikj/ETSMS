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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Role).HasMaxLength(100);

            // Profile fields
            builder.Property(e => e.Department).HasMaxLength(150);
            builder.Property(e => e.JobTitle).HasMaxLength(150);
            builder.Property(e => e.PhoneNumber).HasMaxLength(50);
            builder.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
            builder.Property(e => e.Bio).HasMaxLength(1000);
            builder.Property(e => e.Location).HasMaxLength(200);
        });
    }
}
