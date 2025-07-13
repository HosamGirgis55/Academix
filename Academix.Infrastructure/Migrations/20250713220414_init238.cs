using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init238 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropTable(
                name: "LearningInterests");

            migrationBuilder.DropIndex(
                name: "IX_LearningInterestsStudents_LearningInterestId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropColumn(
                name: "LearningInterestId",
                table: "LearningInterestsStudents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LearningInterestId",
                table: "LearningInterestsStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "LearningInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningInterests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningInterestsStudents_LearningInterestId",
                table: "LearningInterestsStudents",
                column: "LearningInterestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId",
                table: "LearningInterestsStudents",
                column: "LearningInterestId",
                principalTable: "LearningInterests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
