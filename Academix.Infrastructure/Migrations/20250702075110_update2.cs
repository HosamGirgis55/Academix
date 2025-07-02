using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConnectPrograming",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Github",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GraduationStatus",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Level",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SkilleId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialistId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "level",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_level", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProblemSolving",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemSolving", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skille",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skille", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProblemSolvingStudent",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemSolvingStudent", x => new { x.StudentId, x.StudentId1 });
                    table.ForeignKey(
                        name: "FK_ProblemSolvingStudent_ProblemSolving_StudentId",
                        column: x => x.StudentId,
                        principalTable: "ProblemSolving",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemSolvingStudent_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSkiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkilleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSkiles_Skille_SkilleId",
                        column: x => x.SkilleId,
                        principalTable: "Skille",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSkiles_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GraduationStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    studentSkilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraduationStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GraduationStatus_StudentSkiles_studentSkilesId",
                        column: x => x.studentSkilesId,
                        principalTable: "StudentSkiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_GraduationStatus",
                table: "Students",
                column: "GraduationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Level",
                table: "Students",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SkilleId",
                table: "Students",
                column: "SkilleId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SpecialistId",
                table: "Students",
                column: "SpecialistId");

            migrationBuilder.CreateIndex(
                name: "IX_GraduationStatus_studentSkilesId",
                table: "GraduationStatus",
                column: "studentSkilesId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSolvingStudent_StudentId1",
                table: "ProblemSolvingStudent",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkiles_SkilleId",
                table: "StudentSkiles",
                column: "SkilleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkiles_StudentId",
                table: "StudentSkiles",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_GraduationStatus_GraduationStatus",
                table: "Students",
                column: "GraduationStatus",
                principalTable: "GraduationStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Skille_SkilleId",
                table: "Students",
                column: "SkilleId",
                principalTable: "Skille",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Specialist_SpecialistId",
                table: "Students",
                column: "SpecialistId",
                principalTable: "Specialist",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_level_Level",
                table: "Students",
                column: "Level",
                principalTable: "level",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_GraduationStatus_GraduationStatus",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Skille_SkilleId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Specialist_SpecialistId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_level_Level",
                table: "Students");

            migrationBuilder.DropTable(
                name: "GraduationStatus");

            migrationBuilder.DropTable(
                name: "level");

            migrationBuilder.DropTable(
                name: "ProblemSolvingStudent");

            migrationBuilder.DropTable(
                name: "Specialist");

            migrationBuilder.DropTable(
                name: "StudentSkiles");

            migrationBuilder.DropTable(
                name: "ProblemSolving");

            migrationBuilder.DropTable(
                name: "Skille");

            migrationBuilder.DropIndex(
                name: "IX_Students_GraduationStatus",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_Level",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SkilleId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SpecialistId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ConnectPrograming",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Github",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GraduationStatus",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SkilleId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SpecialistId",
                table: "Students");

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
