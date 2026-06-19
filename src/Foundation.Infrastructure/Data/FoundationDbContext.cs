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
    public DbSet<SkillAssessmentSnapshot> SkillAssessmentSnapshots => Set<SkillAssessmentSnapshot>();
    public DbSet<SkillAssessmentEntry> SkillAssessmentEntries => Set<SkillAssessmentEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Role).HasMaxLength(100);
            builder.HasMany(e => e.SkillAssessmentSnapshots)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId);
        });

        modelBuilder.Entity<SkillAssessmentSnapshot>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Timestamp)
                .IsRequired();
            builder.HasMany(s => s.Entries)
                .WithOne(e => e.Snapshot)
                .HasForeignKey(e => e.SnapshotId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SkillAssessmentEntry>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.SkillName)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(e => e.Proficiency)
                .IsRequired();
            builder.Property(e => e.ProficiencyLabel)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(e => e.IsNoExperience)
                .IsRequired();
        });
    }
}
