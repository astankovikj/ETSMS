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
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Role).HasMaxLength(100);
        });

        modelBuilder.Entity<Skill>(builder =>
        {
            builder.HasKey(s => s.SkillId);
            builder.Property(s => s.SkillName).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Category).HasMaxLength(100);
        });

        modelBuilder.Entity<EmployeeSkill>(builder =>
        {
            builder.HasKey(es => new { es.EmployeeId, es.SkillId });

            builder.HasIndex(es => new { es.EmployeeId, es.SkillId }).IsUnique();

            builder.Property(es => es.ProficiencyLevel).IsRequired();
            builder.Property(es => es.Notes).HasMaxLength(500);

            builder.HasOne(es => es.Employee)
                   .WithMany()
                   .HasForeignKey(es => es.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(es => es.Skill)
                   .WithMany()
                   .HasForeignKey(es => es.SkillId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // HR-managed skills catalogue seed data
        modelBuilder.Entity<Skill>().HasData(
            new Skill { SkillId = 1,  SkillName = "TypeScript",       Category = "Frontend" },
            new Skill { SkillId = 2,  SkillName = "JavaScript",        Category = "Frontend" },
            new Skill { SkillId = 3,  SkillName = "React",             Category = "Frontend" },
            new Skill { SkillId = 4,  SkillName = "Vue.js",            Category = "Frontend" },
            new Skill { SkillId = 5,  SkillName = "Angular",           Category = "Frontend" },
            new Skill { SkillId = 6,  SkillName = "C#",                Category = "Backend"  },
            new Skill { SkillId = 7,  SkillName = "PostgreSQL",        Category = "Database" },
            new Skill { SkillId = 8,  SkillName = "Entity Framework",  Category = "Backend"  },
            new Skill { SkillId = 9,  SkillName = "Docker",            Category = "DevOps"   },
            new Skill { SkillId = 10, SkillName = "Kubernetes",        Category = "DevOps"   },
            new Skill { SkillId = 11, SkillName = "Azure",             Category = "Cloud"    },
            new Skill { SkillId = 12, SkillName = "AWS",               Category = "Cloud"    },
            new Skill { SkillId = 13, SkillName = "Python",            Category = "Backend"  },
            new Skill { SkillId = 14, SkillName = "SQL Server",        Category = "Database" },
            new Skill { SkillId = 15, SkillName = "Redis",             Category = "Database" },
            new Skill { SkillId = 16, SkillName = "GraphQL",           Category = "API"      },
            new Skill { SkillId = 17, SkillName = "REST APIs",         Category = "API"      },
            new Skill { SkillId = 18, SkillName = "Git",               Category = "Tooling"  },
            new Skill { SkillId = 19, SkillName = "Terraform",         Category = "DevOps"   },
            new Skill { SkillId = 20, SkillName = "Java",              Category = "Backend"  },
            new Skill { SkillId = 21, SkillName = "Go",                Category = "Backend"  },
            new Skill { SkillId = 22, SkillName = "Rust",              Category = "Backend"  },
            new Skill { SkillId = 23, SkillName = "Node.js",           Category = "Backend"  },
            new Skill { SkillId = 24, SkillName = "MongoDB",           Category = "Database" },
            new Skill { SkillId = 25, SkillName = "Elasticsearch",     Category = "Database" }
        );
    }
}
