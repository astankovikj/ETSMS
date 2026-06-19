using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Foundation.Infrastructure.Data.Migrations;

public partial class AddSkillAssessmentSnapshotAndEntry : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SkillAssessmentSnapshots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SkillAssessmentSnapshots", x => x.Id);
                table.ForeignKey(
                    name: "FK_SkillAssessmentSnapshots_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SkillAssessmentEntries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SnapshotId = table.Column<Guid>(type: "uuid", nullable: false),
                SkillName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                Proficiency = table.Column<int>(type: "integer", nullable: false),
                ProficiencyLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                IsNoExperience = table.Column<bool>(type: "boolean", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SkillAssessmentEntries", x => x.Id);
                table.ForeignKey(
                    name: "FK_SkillAssessmentEntries_SkillAssessmentSnapshots_SnapshotId",
                    column: x => x.SnapshotId,
                    principalTable: "SkillAssessmentSnapshots",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SkillAssessmentEntries_SnapshotId",
            table: "SkillAssessmentEntries",
            column: "SnapshotId");

        migrationBuilder.CreateIndex(
            name: "IX_SkillAssessmentSnapshots_EmployeeId",
            table: "SkillAssessmentSnapshots",
            column: "EmployeeId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "SkillAssessmentEntries");
        migrationBuilder.DropTable(name: "SkillAssessmentSnapshots");
    }
}
