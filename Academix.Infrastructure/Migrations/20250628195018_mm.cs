using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Academix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("2f2f4c4d-a337-4d16-96c9-af083eb3d0e8"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("d05afe3f-741a-48f2-9487-d0738b2851e1"));

            migrationBuilder.CreateTable(
                name: "Nationalities",
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
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
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
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("962b78c0-e608-41c8-9a7f-b44f47d06996"), new DateTime(2025, 6, 28, 19, 50, 17, 706, DateTimeKind.Utc).AddTicks(9230), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null },
                    { new Guid("b14273fc-8775-4c48-ac4c-03320317b6bd"), new DateTime(2025, 6, 28, 19, 50, 17, 706, DateTimeKind.Utc).AddTicks(9235), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("962b78c0-e608-41c8-9a7f-b44f47d06996"));

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: new Guid("b14273fc-8775-4c48-ac4c-03320317b6bd"));

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "Email", "FirstName", "FirstNameAr", "LastName", "LastNameAr", "StudentNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("2f2f4c4d-a337-4d16-96c9-af083eb3d0e8"), new DateTime(2025, 6, 28, 14, 25, 29, 608, DateTimeKind.Utc).AddTicks(8567), null, new DateTime(2001, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jane.smith@example.com", "Jane", "جين", "Smith", "سميث", "20240002", null, null },
                    { new Guid("d05afe3f-741a-48f2-9487-d0738b2851e1"), new DateTime(2025, 6, 28, 14, 25, 29, 608, DateTimeKind.Utc).AddTicks(8562), null, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@example.com", "John", "جون", "Doe", "دو", "20240001", null, null }
                });
        }
    }
}
