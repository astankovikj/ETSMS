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
    public DbSet<TechnicalSkill> TechnicalSkills => Set<TechnicalSkill>();
    public DbSet<DomainExpertise> DomainExpertise => Set<DomainExpertise>();
    public DbSet<Certification> Certifications => Set<Certification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Role).HasMaxLength(100);
        });

        modelBuilder.Entity<TechnicalSkill>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ProficiencyLevel).IsRequired();
            builder.HasOne(s => s.Employee)
                   .WithMany()
                   .HasForeignKey(s => s.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DomainExpertise>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
            builder.HasOne(d => d.Employee)
                   .WithMany()
                   .HasForeignKey(d => d.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Certification>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(300);
            builder.HasOne(c => c.Employee)
                   .WithMany()
                   .HasForeignKey(c => c.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
