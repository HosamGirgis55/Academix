using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.AlterColumn<Guid>(
                name: "LearningInterestId1",
                table: "LearningInterestsStudents",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents",
                column: "LearningInterestId1",
                principalTable: "LearningInterests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.AlterColumn<Guid>(
                name: "LearningInterestId1",
                table: "LearningInterestsStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents",
                column: "LearningInterestId1",
                principalTable: "LearningInterests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
