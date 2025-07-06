using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("9540339f-e617-4623-8208-7ed7614eda54"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("f8d283b5-92ef-4b92-9376-a17a47ef1d8b"));

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameArabic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("2f2f4c4d-a337-4d16-96c9-af083eb3d0e8"), new DateTime(2025, 6, 28, 14, 25, 29, 608, DateTimeKind.Utc).AddTicks(8567), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null },
                    { new Guid("d05afe3f-741a-48f2-9487-d0738b2851e1"), new DateTime(2025, 6, 28, 14, 25, 29, 608, DateTimeKind.Utc).AddTicks(8562), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("2f2f4c4d-a337-4d16-96c9-af083eb3d0e8"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("d05afe3f-741a-48f2-9487-d0738b2851e1"));

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("9540339f-e617-4623-8208-7ed7614eda54"), new DateTime(2025, 6, 20, 20, 54, 53, 641, DateTimeKind.Utc).AddTicks(1933), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null },
                    { new Guid("f8d283b5-92ef-4b92-9376-a17a47ef1d8b"), new DateTime(2025, 6, 20, 20, 54, 53, 641, DateTimeKind.Utc).AddTicks(1939), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null }
                });
        }
    }
}
