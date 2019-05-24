using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backendProject.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Issuer = table.Column<string>(nullable: false),
                    SubjectId = table.Column<string>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => new { x.SubjectId, x.Issuer });
                    table.UniqueConstraint("AK_Identity_UniqueId", x => x.UniqueId);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    UniqueId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.UniqueId);
                    table.ForeignKey(
                        name: "FK_Admin_Identity_UniqueId",
                        column: x => x.UniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    UniqueId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Picture = table.Column<string>(nullable: true),
                    BirthDate = table.Column<long>(nullable: true),
                    Gender = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.UniqueId);
                    table.ForeignKey(
                        name: "FK_Profile_Identity_UniqueId",
                        column: x => x.UniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Token = table.Column<Guid>(nullable: false),
                    IssuedUtc = table.Column<DateTime>(nullable: false),
                    ExpiresUtc = table.Column<DateTime>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    IdentitySubjectId = table.Column<string>(nullable: true),
                    IdentityIssuer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Identity_IdentitySubjectId_IdentityIssuer",
                        columns: x => new { x.IdentitySubjectId, x.IdentityIssuer },
                        principalTable: "Identity",
                        principalColumns: new[] { "SubjectId", "Issuer" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IdentitySubjectId_IdentityIssuer",
                table: "RefreshToken",
                columns: new[] { "IdentitySubjectId", "IdentityIssuer" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Identity");
        }
    }
}
