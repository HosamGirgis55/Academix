using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TeachingLanguages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TeachingAreas");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CommunicationMethods");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "GraduationStatuses",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "GraduationStatuses",
                newName: "NameAr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "GraduationStatuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NameAr",
                table: "GraduationStatuses",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TeachingLanguages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TeachingAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Specializations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CommunicationMethods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
