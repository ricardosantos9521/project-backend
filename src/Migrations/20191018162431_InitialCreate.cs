using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.Backend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Issuer = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 100, nullable: false),
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
                    BirthDate = table.Column<DateTime>(nullable: true),
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
                name: "Session",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    FirstLogin = table.Column<DateTime>(nullable: false),
                    LastLogin = table.Column<DateTime>(nullable: false),
                    IdentityUniqueId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Session_Identity_IdentityUniqueId",
                        column: x => x.IdentityUniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    FileId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    FileLength = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: false),
                    Bytes = table.Column<byte[]>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    OwnedByUniqueId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_File_Profile_OwnedByUniqueId",
                        column: x => x.OwnedByUniqueId,
                        principalTable: "Profile",
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
                    SessionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadPermissions",
                columns: table => new
                {
                    FileId = table.Column<Guid>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    SharedByUniqueId = table.Column<Guid>(nullable: false),
                    SharedByIdentityUniqueId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadPermissions", x => new { x.FileId, x.UniqueId });
                    table.ForeignKey(
                        name: "FK_ReadPermissions_File_FileId",
                        column: x => x.FileId,
                        principalTable: "File",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReadPermissions_Identity_SharedByIdentityUniqueId",
                        column: x => x.SharedByIdentityUniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReadPermissions_Identity_UniqueId",
                        column: x => x.UniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritePermissions",
                columns: table => new
                {
                    FileId = table.Column<Guid>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    SharedByUniqueId = table.Column<Guid>(nullable: false),
                    SharedByIdentityUniqueId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritePermissions", x => new { x.FileId, x.UniqueId });
                    table.ForeignKey(
                        name: "FK_WritePermissions_File_FileId",
                        column: x => x.FileId,
                        principalTable: "File",
                        principalColumn: "FileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritePermissions_Identity_SharedByIdentityUniqueId",
                        column: x => x.SharedByIdentityUniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WritePermissions_Identity_UniqueId",
                        column: x => x.UniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_File_OwnedByUniqueId",
                table: "File",
                column: "OwnedByUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadPermissions_SharedByIdentityUniqueId",
                table: "ReadPermissions",
                column: "SharedByIdentityUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadPermissions_UniqueId",
                table: "ReadPermissions",
                column: "UniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_SessionId",
                table: "RefreshToken",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Session_IdentityUniqueId",
                table: "Session",
                column: "IdentityUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_WritePermissions_SharedByIdentityUniqueId",
                table: "WritePermissions",
                column: "SharedByIdentityUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_WritePermissions_UniqueId",
                table: "WritePermissions",
                column: "UniqueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "ReadPermissions");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "WritePermissions");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Identity");
        }
    }
}
