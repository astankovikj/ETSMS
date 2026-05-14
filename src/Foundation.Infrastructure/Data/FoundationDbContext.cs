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
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();
    public DbSet<Domain> Domains => Set<Domain>();
    public DbSet<EmployeeDomain> EmployeeDomains => Set<EmployeeDomain>();
    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<EmployeeCertification> EmployeeCertifications => Set<EmployeeCertification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Employee ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(250);
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Role).HasMaxLength(100);
            builder.Property(e => e.Department).HasMaxLength(100);
        });

        // ── SkillCategory ─────────────────────────────────────────────────────
        modelBuilder.Entity<SkillCategory>(builder =>
        {
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Name).IsRequired().HasMaxLength(100);
        });

        // ── Skill ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Skill>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
            builder.HasOne(s => s.Category)
                   .WithMany(sc => sc.Skills)
                   .HasForeignKey(s => s.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // ── EmployeeSkill ─────────────────────────────────────────────────────
        modelBuilder.Entity<EmployeeSkill>(builder =>
        {
            builder.HasKey(es => new { es.EmployeeId, es.SkillId });
            builder.HasOne(es => es.Employee)
                   .WithMany(e => e.EmployeeSkills)
                   .HasForeignKey(es => es.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(es => es.Skill)
                   .WithMany(s => s.EmployeeSkills)
                   .HasForeignKey(es => es.SkillId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ProficiencyLevel must be between 1 and 5 at the database layer
            builder.ToTable(t =>
                t.HasCheckConstraint("CK_EmployeeSkill_ProficiencyLevel",
                    "\"ProficiencyLevel\" >= 1 AND \"ProficiencyLevel\" <= 5"));
        });

        // ── Domain ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Domain>(builder =>
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
        });

        // ── EmployeeDomain ────────────────────────────────────────────────────
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
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Certification ─────────────────────────────────────────────────────
        modelBuilder.Entity<Certification>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        });

        // ── EmployeeCertification ─────────────────────────────────────────────
        modelBuilder.Entity<EmployeeCertification>(builder =>
        {
            builder.HasKey(ec => new { ec.EmployeeId, ec.CertificationId });
            builder.HasOne(ec => ec.Employee)
                   .WithMany(e => e.EmployeeCertifications)
                   .HasForeignKey(ec => ec.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ec => ec.Certification)
                   .WithMany(c => c.EmployeeCertifications)
                   .HasForeignKey(ec => ec.CertificationId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Seed data ─────────────────────────────────────────────────────────
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Skill categories
        var catFrontend  = new Guid("00000000-0000-0000-0001-000000000001");
        var catBackend   = new Guid("00000000-0000-0000-0001-000000000002");
        var catCloud     = new Guid("00000000-0000-0000-0001-000000000003");
        var catData      = new Guid("00000000-0000-0000-0001-000000000004");

        modelBuilder.Entity<SkillCategory>().HasData(
            new SkillCategory { Id = catFrontend, Name = "Frontend" },
            new SkillCategory { Id = catBackend,  Name = "Backend"  },
            new SkillCategory { Id = catCloud,    Name = "Cloud"    },
            new SkillCategory { Id = catData,     Name = "Data"     }
        );

        // Skills
        var skillReact      = new Guid("00000000-0000-0000-0002-000000000001");
        var skillTypeScript = new Guid("00000000-0000-0000-0002-000000000002");
        var skillCSharp     = new Guid("00000000-0000-0000-0002-000000000003");
        var skillDotNet     = new Guid("00000000-0000-0000-0002-000000000004");
        var skillAzure      = new Guid("00000000-0000-0000-0002-000000000005");
        var skillAws        = new Guid("00000000-0000-0000-0002-000000000006");
        var skillSql        = new Guid("00000000-0000-0000-0002-000000000007");
        var skillPython     = new Guid("00000000-0000-0000-0002-000000000008");

        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = skillReact,      Name = "React",      CategoryId = catFrontend },
            new Skill { Id = skillTypeScript, Name = "TypeScript", CategoryId = catFrontend },
            new Skill { Id = skillCSharp,     Name = "C#",         CategoryId = catBackend  },
            new Skill { Id = skillDotNet,     Name = ".NET",       CategoryId = catBackend  },
            new Skill { Id = skillAzure,      Name = "Azure",      CategoryId = catCloud    },
            new Skill { Id = skillAws,        Name = "AWS",        CategoryId = catCloud    },
            new Skill { Id = skillSql,        Name = "SQL",        CategoryId = catData     },
            new Skill { Id = skillPython,     Name = "Python",     CategoryId = catData     }
        );

        // Domains
        var domainFinTech   = new Guid("00000000-0000-0000-0003-000000000001");
        var domainHealthcare = new Guid("00000000-0000-0000-0003-000000000002");
        var domainRetail    = new Guid("00000000-0000-0000-0003-000000000003");
        var domainLogistics = new Guid("00000000-0000-0000-0003-000000000004");

        modelBuilder.Entity<Domain>().HasData(
            new Domain { Id = domainFinTech,    Name = "FinTech"     },
            new Domain { Id = domainHealthcare, Name = "Healthcare"  },
            new Domain { Id = domainRetail,     Name = "Retail"      },
            new Domain { Id = domainLogistics,  Name = "Logistics"   }
        );

        // Certifications
        var certAz900  = new Guid("00000000-0000-0000-0004-000000000001");
        var certAz204  = new Guid("00000000-0000-0000-0004-000000000002");
        var certAwsSaa = new Guid("00000000-0000-0000-0004-000000000003");
        var certCkad   = new Guid("00000000-0000-0000-0004-000000000004");

        modelBuilder.Entity<Certification>().HasData(
            new Certification { Id = certAz900,  Name = "Microsoft Azure Fundamentals (AZ-900)"         },
            new Certification { Id = certAz204,  Name = "Microsoft Azure Developer Associate (AZ-204)"  },
            new Certification { Id = certAwsSaa, Name = "AWS Solutions Architect Associate"              },
            new Certification { Id = certCkad,   Name = "Certified Kubernetes Application Developer"    }
        );
    }
}
