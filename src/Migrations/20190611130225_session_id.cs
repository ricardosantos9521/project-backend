using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backendProject.Migrations
{
    public partial class session_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Identity_IdentitySubjectId_IdentityIssuer",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_IdentitySubjectId_IdentityIssuer",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "IdentityIssuer",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "IdentitySubjectId",
                table: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                table: "RefreshToken",
                newName: "SessionId");

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Session_Identity_UniqueId",
                        column: x => x.UniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_SessionId",
                table: "RefreshToken",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Session_UniqueId",
                table: "Session",
                column: "UniqueId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Session_SessionId",
                table: "RefreshToken",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Session_SessionId",
                table: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_SessionId",
                table: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "RefreshToken",
                newName: "UniqueId");

            migrationBuilder.AddColumn<string>(
                name: "IdentityIssuer",
                table: "RefreshToken",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentitySubjectId",
                table: "RefreshToken",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IdentitySubjectId_IdentityIssuer",
                table: "RefreshToken",
                columns: new[] { "IdentitySubjectId", "IdentityIssuer" });

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Identity_IdentitySubjectId_IdentityIssuer",
                table: "RefreshToken",
                columns: new[] { "IdentitySubjectId", "IdentityIssuer" },
                principalTable: "Identity",
                principalColumns: new[] { "SubjectId", "Issuer" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
