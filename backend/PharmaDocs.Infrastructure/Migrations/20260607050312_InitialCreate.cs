using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PharmaDocs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DIN = table.Column<string>(type: "text", nullable: true),
                    NPN = table.Column<string>(type: "text", nullable: true),
                    MedicinalIngredient = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    DosageForm = table.Column<string>(type: "text", nullable: false),
                    RouteOfAdministration = table.Column<string>(type: "text", nullable: false),
                    TherapeuticCategory = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Users_ArchivedById",
                        column: x => x.ArchivedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentRecords_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentRecords_Users_ArchivedById",
                        column: x => x.ArchivedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentRecords_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionType = table.Column<string>(type: "text", nullable: false),
                    RegulatoryBody = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SubmissionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedById = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionPackages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionPackages_Users_ArchivedById",
                        column: x => x.ArchivedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionPackages_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    OldStatus = table.Column<string>(type: "text", nullable: false),
                    NewStatus = table.Column<string>(type: "text", nullable: false),
                    ChangedByName = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmissionPackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChangedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_SubmissionPackages_SubmissionPackageId",
                        column: x => x.SubmissionPackageId,
                        principalTable: "SubmissionPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionDocuments",
                columns: table => new
                {
                    SubmissionPackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionDocuments", x => new { x.SubmissionPackageId, x.DocumentRecordId });
                    table.ForeignKey(
                        name: "FK_SubmissionDocuments_DocumentRecords_DocumentRecordId",
                        column: x => x.DocumentRecordId,
                        principalTable: "DocumentRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionDocuments_SubmissionPackages_SubmissionPackageId",
                        column: x => x.SubmissionPackageId,
                        principalTable: "SubmissionPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sarah@pharmadocs.ca", "Sarah Leblanc", true, "$2a$11$gC76SgMbnnNGOHdy6WKR/uaAL2ZkBonnlNbdr5M/bLYCb8C1NTGBu", "RegAffairsOfficer" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "james@pharmadocs.ca", "James Okafor", true, "$2a$11$gC76SgMbnnNGOHdy6WKR/uaAL2ZkBonnlNbdr5M/bLYCb8C1NTGBu", "Viewer" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ArchivedAt", "ArchivedById", "CreatedAt", "CreatedById", "DIN", "DosageForm", "IsArchived", "Manufacturer", "MedicinalIngredient", "NPN", "Name", "RouteOfAdministration", "TherapeuticCategory" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, null, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "02245276", "Tablet", false, "Apotex Inc.", "Atorvastatin Calcium", null, "Atorvastatin 20mg Tablet", "Oral", "Cardiovascular" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, null, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), "02162512", "Tablet", false, "Teva Canada", "Metformin Hydrochloride", null, "Metformin 500mg Tablet", "Oral", "Antidiabetic" }
                });

            migrationBuilder.InsertData(
                table: "DocumentRecords",
                columns: new[] { "Id", "ArchivedAt", "ArchivedById", "CreatedAt", "CreatedById", "Date", "IsArchived", "Notes", "ProductId", "Status", "Title", "Type", "Version" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateOnly(2026, 1, 20), false, "Initial approved monograph", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Final", "Atorvastatin Product Monograph v1.0", "ProductMonograph", "1.0" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, null, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new DateOnly(2026, 2, 1), false, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Draft", "Metformin Certificate of Analysis v1.0", "CertificateOfAnalysis", "1.0" }
                });

            migrationBuilder.InsertData(
                table: "SubmissionPackages",
                columns: new[] { "Id", "ArchivedAt", "ArchivedById", "CreatedAt", "CreatedById", "IsArchived", "ProductId", "RegulatoryBody", "Status", "SubmissionDate", "SubmissionType", "TargetDate" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), null, null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), false, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Health Canada", "Draft", null, "NDS", new DateOnly(2026, 6, 30) });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedById",
                table: "AuditLogs",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_SubmissionPackageId",
                table: "AuditLogs",
                column: "SubmissionPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRecords_ArchivedById",
                table: "DocumentRecords",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRecords_CreatedById",
                table: "DocumentRecords",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRecords_ProductId",
                table: "DocumentRecords",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ArchivedById",
                table: "Products",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedById",
                table: "Products",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionDocuments_DocumentRecordId",
                table: "SubmissionDocuments",
                column: "DocumentRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPackages_ArchivedById",
                table: "SubmissionPackages",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPackages_CreatedById",
                table: "SubmissionPackages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPackages_ProductId",
                table: "SubmissionPackages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "SubmissionDocuments");

            migrationBuilder.DropTable(
                name: "DocumentRecords");

            migrationBuilder.DropTable(
                name: "SubmissionPackages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
