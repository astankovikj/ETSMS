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
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
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

        modelBuilder.Entity<SkillCategory>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<TechnicalSkill>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Proficiency).IsRequired();
            builder.Property(s => s.LastUpdated).IsRequired();

            builder.HasOne(s => s.Employee)
                   .WithMany()
                   .HasForeignKey(s => s.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Category)
                   .WithMany(c => c.TechnicalSkills)
                   .HasForeignKey(s => s.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
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
            builder.Property(c => c.ExpirationDate).IsRequired();

            builder.HasOne(c => c.Employee)
                   .WithMany()
                   .HasForeignKey(c => c.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
