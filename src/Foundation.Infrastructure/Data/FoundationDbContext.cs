using Foundation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Foundation.Domain.Entities.Domain;

namespace Foundation.Infrastructure.Data;

public sealed class FoundationDbContext : DbContext
{
    public FoundationDbContext(DbContextOptions<FoundationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<TechnologyRelationship> TechnologyRelationships => Set<TechnologyRelationship>();
    public DbSet<DomainEntity> Domains => Set<DomainEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Role).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Technology>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(200);

            builder.HasOne(t => t.Category)
                   .WithMany(c => c.Technologies)
                   .HasForeignKey(t => t.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TechnologyRelationship>(builder =>
        {
            builder.HasKey(r => new { r.PrimaryTechnologyId, r.SecondaryTechnologyId });

            builder.HasOne(r => r.PrimaryTechnology)
                   .WithMany(t => t.PrimaryRelationships)
                   .HasForeignKey(r => r.PrimaryTechnologyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.SecondaryTechnology)
                   .WithMany(t => t.SecondaryRelationships)
                   .HasForeignKey(r => r.SecondaryTechnologyId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DomainEntity>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            builder.Property(d => d.DomainType).IsRequired();
        });
    }
}
