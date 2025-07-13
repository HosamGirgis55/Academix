using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_Fields_LearningInterestId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Nationalities_NationalityId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Nationalities_NationalityId1",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Countries_NationalityId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_NationalityId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_NationalityId1",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningInterestsStudents",
                table: "LearningInterestsStudents");

            migrationBuilder.DropIndex(
                name: "IX_LearningInterestsStudents_LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.DropColumn(
                name: "NationalityId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "NationalityId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LearningInterestId1",
                table: "LearningInterestsStudents");

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "LearningInterestsStudents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningInterestsStudents",
                table: "LearningInterestsStudents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LearningInterestsStudents_FieldId",
                table: "LearningInterestsStudents",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningInterestsStudents_StudentId",
                table: "LearningInterestsStudents",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_Fields_FieldId",
                table: "LearningInterestsStudents",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId",
                table: "LearningInterestsStudents",
                column: "LearningInterestId",
                principalTable: "LearningInterests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Nationalities_NationalityId",
                table: "Students",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_Fields_FieldId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningInterestsStudents_LearningInterests_LearningInterestId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Nationalities_NationalityId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningInterestsStudents",
                table: "LearningInterestsStudents");

            migrationBuilder.DropIndex(
                name: "IX_LearningInterestsStudents_FieldId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropIndex(
                name: "IX_LearningInterestsStudents_StudentId",
                table: "LearningInterestsStudents");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "LearningInterestsStudents");

            migrationBuilder.AddColumn<Guid>(
                name: "NationalityId",
                table: "Teachers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NationalityId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LearningInterestId1",
                table: "LearningInterestsStudents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningInterestsStudents",
                table: "LearningInterestsStudents",
                columns: new[] { "StudentId", "LearningInterestId" });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_NationalityId",
                table: "Teachers",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_NationalityId1",
                table: "Students",
                column: "NationalityId1");

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
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Nationalities_NationalityId",
                table: "Students",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Nationalities_NationalityId1",
                table: "Students",
                column: "NationalityId1",
                principalTable: "Nationalities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Countries_NationalityId",
                table: "Teachers",
                column: "NationalityId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
