using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editRelation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NationalityId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_NationalityId1",
                table: "Students",
                column: "NationalityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Nationalities_NationalityId1",
                table: "Students",
                column: "NationalityId1",
                principalTable: "Nationalities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Nationalities_NationalityId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_NationalityId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "NationalityId1",
                table: "Students");
        }
    }
}
