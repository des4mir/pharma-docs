using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaDocs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandAuditLogAndArchiveEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_SubmissionPackages_SubmissionPackageId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_ChangedById",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentRecords_Users_CreatedById",
                table: "DocumentRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatedById",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPackages_Users_CreatedById",
                table: "SubmissionPackages");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "SubmissionPackages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ArchivedById",
                table: "SubmissionPackages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "SubmissionPackages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "SubmissionPackages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ArchivedById",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "DocumentRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ArchivedById",
                table: "DocumentRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "DocumentRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "DocumentRecords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubmissionPackageId",
                table: "AuditLogs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "AuditLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValues",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DocumentRecords",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "ArchivedAt", "ArchivedById", "IsArchived", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "DocumentRecords",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "ArchivedAt", "ArchivedById", "IsArchived", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "ArchivedAt", "ArchivedById", "IsArchived", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "ArchivedAt", "ArchivedById", "IsArchived", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "SubmissionPackages",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                columns: new[] { "ArchivedAt", "ArchivedById", "IsArchived", "UserId" },
                values: new object[] { null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "IsActive",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPackages_ArchivedById",
                table: "SubmissionPackages",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPackages_UserId",
                table: "SubmissionPackages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ArchivedById",
                table: "Products",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId",
                table: "Products",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRecords_ArchivedById",
                table: "DocumentRecords",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRecords_UserId",
                table: "DocumentRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_SubmissionPackages_SubmissionPackageId",
                table: "AuditLogs",
                column: "SubmissionPackageId",
                principalTable: "SubmissionPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_ChangedById",
                table: "AuditLogs",
                column: "ChangedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentRecords_Users_ArchivedById",
                table: "DocumentRecords",
                column: "ArchivedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentRecords_Users_CreatedById",
                table: "DocumentRecords",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentRecords_Users_UserId",
                table: "DocumentRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_ArchivedById",
                table: "Products",
                column: "ArchivedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatedById",
                table: "Products",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserId",
                table: "Products",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPackages_Users_ArchivedById",
                table: "SubmissionPackages",
                column: "ArchivedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPackages_Users_CreatedById",
                table: "SubmissionPackages",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPackages_Users_UserId",
                table: "SubmissionPackages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_SubmissionPackages_SubmissionPackageId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_ChangedById",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentRecords_Users_ArchivedById",
                table: "DocumentRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentRecords_Users_CreatedById",
                table: "DocumentRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentRecords_Users_UserId",
                table: "DocumentRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_ArchivedById",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_CreatedById",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPackages_Users_ArchivedById",
                table: "SubmissionPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPackages_Users_CreatedById",
                table: "SubmissionPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPackages_Users_UserId",
                table: "SubmissionPackages");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionPackages_ArchivedById",
                table: "SubmissionPackages");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionPackages_UserId",
                table: "SubmissionPackages");

            migrationBuilder.DropIndex(
                name: "IX_Products_ArchivedById",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_DocumentRecords_ArchivedById",
                table: "DocumentRecords");

            migrationBuilder.DropIndex(
                name: "IX_DocumentRecords_UserId",
                table: "DocumentRecords");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "SubmissionPackages");

            migrationBuilder.DropColumn(
                name: "ArchivedById",
                table: "SubmissionPackages");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "SubmissionPackages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SubmissionPackages");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ArchivedById",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "DocumentRecords");

            migrationBuilder.DropColumn(
                name: "ArchivedById",
                table: "DocumentRecords");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "DocumentRecords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DocumentRecords");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OldValues",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubmissionPackageId",
                table: "AuditLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_SubmissionPackages_SubmissionPackageId",
                table: "AuditLogs",
                column: "SubmissionPackageId",
                principalTable: "SubmissionPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_ChangedById",
                table: "AuditLogs",
                column: "ChangedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentRecords_Users_CreatedById",
                table: "DocumentRecords",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_CreatedById",
                table: "Products",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPackages_Users_CreatedById",
                table: "SubmissionPackages",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
