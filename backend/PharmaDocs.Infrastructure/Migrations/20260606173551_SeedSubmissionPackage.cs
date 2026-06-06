using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaDocs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSubmissionPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SubmissionPackages",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "ProductId", "RegulatoryBody", "Status", "SubmissionDate", "SubmissionType", "TargetDate" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Health Canada", "Draft", null, "NDS", new DateOnly(2026, 6, 30) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubmissionPackages",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));
        }
    }
}
