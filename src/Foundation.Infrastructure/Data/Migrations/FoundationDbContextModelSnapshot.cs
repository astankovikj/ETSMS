using System;
using Foundation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Foundation.Infrastructure.Data.Migrations;

[DbContext(typeof(FoundationDbContext))]
partial class FoundationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("Foundation.Domain.Entities.Employee", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("timestamp without time zone");

            b.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnType("character varying(250)");

            b.Property<string>("FirstName")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("LastName")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Role")
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            b.HasKey("Id");

            b.HasMany("Foundation.Domain.Entities.SkillAssessmentSnapshot", "SkillAssessmentSnapshots")
                .WithOne("Employee")
                .HasForeignKey("EmployeeId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.ToTable("Employees");
        });

        modelBuilder.Entity("Foundation.Domain.Entities.SkillAssessmentEntry", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid");

            b.Property<bool>("IsNoExperience")
                .HasColumnType("boolean");

            b.Property<string>("Notes")
                .HasColumnType("text");

            b.Property<int>("Proficiency")
                .HasColumnType("integer");

            b.Property<string>("ProficiencyLabel")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            b.Property<Guid>("SnapshotId")
                .HasColumnType("uuid");

            b.Property<string>("SkillName")
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnType("character varying(250)");

            b.HasKey("Id");

            b.HasIndex("SnapshotId");

            b.ToTable("SkillAssessmentEntries");
        });

        modelBuilder.Entity("Foundation.Domain.Entities.SkillAssessmentSnapshot", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("uuid");

            b.Property<DateTime>("Timestamp")
                .HasColumnType("timestamp without time zone");

            b.Property<Guid>("EmployeeId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("EmployeeId");

            b.ToTable("SkillAssessmentSnapshots");
        });

        modelBuilder.Entity("Foundation.Domain.Entities.SkillAssessmentEntry", b =>
        {
            b.HasOne("Foundation.Domain.Entities.SkillAssessmentSnapshot", "Snapshot")
                .WithMany("Entries")
                .HasForeignKey("SnapshotId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Foundation.Domain.Entities.SkillAssessmentSnapshot", b =>
        {
            b.HasOne("Foundation.Domain.Entities.Employee", "Employee")
                .WithMany("SkillAssessmentSnapshots")
                .HasForeignKey("EmployeeId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
    }
}
