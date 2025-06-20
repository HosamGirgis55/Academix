using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init2o : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("6c608ce3-7295-4aba-8279-4f99014f8887"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("b4e81b97-d442-4f0f-a2b8-56605edb777c"));

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("9540339f-e617-4623-8208-7ed7614eda54"), new DateTime(2025, 6, 20, 20, 54, 53, 641, DateTimeKind.Utc).AddTicks(1933), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null },
                    { new Guid("f8d283b5-92ef-4b92-9376-a17a47ef1d8b"), new DateTime(2025, 6, 20, 20, 54, 53, 641, DateTimeKind.Utc).AddTicks(1939), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("9540339f-e617-4623-8208-7ed7614eda54"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("f8d283b5-92ef-4b92-9376-a17a47ef1d8b"));

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("6c608ce3-7295-4aba-8279-4f99014f8887"), new DateTime(2025, 6, 20, 20, 41, 6, 898, DateTimeKind.Utc).AddTicks(1558), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null },
                    { new Guid("b4e81b97-d442-4f0f-a2b8-56605edb777c"), new DateTime(2025, 6, 20, 20, 41, 6, 898, DateTimeKind.Utc).AddTicks(1517), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null }
                });
        }
    }
}
