using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editLearningInterestes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId",
                table: "LearningInterestsStudents");

            migrationBuilder.AddColumn<Guid>(
                name: "LearningInterestId1",
                table: "LearningInterestsStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LearningInterestsStudents_LearningInterestId1",
                table: "LearningInterestsStudents",
                column: "LearningInterestId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_Fields_LearningInterestId",
                table: "LearningInterestsStudents",
                column: "LearningInterestId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents",
                column: "LearningInterestId1",
                principalTable: "LearningInterests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_Fields_LearningInterestId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.DropIndex(
                name: "IX_LearningInterestsStudents_LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.DropColumn(
                name: "LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId",
                table: "LearningInterestsStudents",
                column: "LearningInterestId",
                principalTable: "LearningInterests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
