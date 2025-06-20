using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("2b6f6ceb-58a5-4279-a60c-88c72202b277"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("a597a791-eec0-4099-8d9b-400d03acb2c0"));

            migrationBuilder.AddColumn<string>(
                name: "FirstNameAr",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastNameAr",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("6c608ce3-7295-4aba-8279-4f99014f8887"), new DateTime(2025, 6, 20, 20, 41, 6, 898, DateTimeKind.Utc).AddTicks(1558), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null },
                    { new Guid("b4e81b97-d442-4f0f-a2b8-56605edb777c"), new DateTime(2025, 6, 20, 20, 41, 6, 898, DateTimeKind.Utc).AddTicks(1517), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("6c608ce3-7295-4aba-8279-4f99014f8887"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("b4e81b97-d442-4f0f-a2b8-56605edb777c"));

            migrationBuilder.DropColumn(
                name: "FirstNameAr",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastNameAr",
                table: "Students");

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "LastName", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("2b6f6ceb-58a5-4279-a60c-88c72202b277"), new DateTime(2025, 6, 20, 20, 15, 37, 553, DateTimeKind.Utc).AddTicks(9975), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "Smith", "20240002", null, null },
                    { new Guid("a597a791-eec0-4099-8d9b-400d03acb2c0"), new DateTime(2025, 6, 20, 20, 15, 37, 553, DateTimeKind.Utc).AddTicks(9968), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "Doe", "20240001", null, null }
                });
        }
    }
}
