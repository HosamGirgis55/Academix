using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningInterestsToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preferences",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "AspNetUsers",
                newName: "Gender");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ConnectProgramming",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Github",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "GraduationStatusId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GraduationStatusId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningInterests",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LevelId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LevelId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NationalityId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ResidenceCountryId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialistId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SpecializationId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraduationStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraduationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentExperiences",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperienceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExperiences", x => new { x.StudentId, x.ExperienceId });
                    table.ForeignKey(
                        name: "FK_StudentExperiences_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentExperiences_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSkills",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkills", x => new { x.StudentId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_StudentSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSkills_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_GraduationStatusId",
                table: "Students",
                column: "GraduationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GraduationStatusId1",
                table: "Students",
                column: "GraduationStatusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Students_LevelId",
                table: "Students",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_LevelId1",
                table: "Students",
                column: "LevelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Students_NationalityId",
                table: "Students",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ResidenceCountryId",
                table: "Students",
                column: "ResidenceCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SpecialistId",
                table: "Students",
                column: "SpecialistId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SpecializationId",
                table: "Students",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExperiences_ExperienceId",
                table: "StudentExperiences",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_SkillId",
                table: "StudentSkills",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Countries_NationalityId",
                table: "Students",
                column: "NationalityId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Countries_ResidenceCountryId",
                table: "Students",
                column: "ResidenceCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_GraduationStatuses_GraduationStatusId",
                table: "Students",
                column: "GraduationStatusId",
                principalTable: "GraduationStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_GraduationStatuses_GraduationStatusId1",
                table: "Students",
                column: "GraduationStatusId1",
                principalTable: "GraduationStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Levels_LevelId",
                table: "Students",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Levels_LevelId1",
                table: "Students",
                column: "LevelId1",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Specializations_SpecialistId",
                table: "Students",
                column: "SpecialistId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Specializations_SpecializationId",
                table: "Students",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Countries_NationalityId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Countries_ResidenceCountryId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_GraduationStatuses_GraduationStatusId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_GraduationStatuses_GraduationStatusId1",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Levels_LevelId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Levels_LevelId1",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Specializations_SpecialistId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Specializations_SpecializationId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "GraduationStatuses");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "StudentExperiences");

            migrationBuilder.DropTable(
                name: "StudentSkills");

            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Students_GraduationStatusId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GraduationStatusId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_LevelId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_LevelId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_NationalityId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ResidenceCountryId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SpecialistId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SpecializationId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ConnectProgramming",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Github",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GraduationStatusId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GraduationStatusId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LearningInterests",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LevelId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "NationalityId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ResidenceCountryId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SpecialistId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "AspNetUsers",
                newName: "gender");

            migrationBuilder.AddColumn<string>(
                name: "Preferences",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
