using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.Backend.Migrations
{
    public partial class files : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Issuer",
                table: "Identity",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Identity",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

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
                name: "ReadPermissions",
                columns: table => new
                {
                    FileId = table.Column<Guid>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    SharedByUniqueId = table.Column<Guid>(nullable: false)
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
                        name: "FK_ReadPermissions_Identity_SharedByUniqueId",
                        column: x => x.SharedByUniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
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
                    SharedByUniqueId = table.Column<Guid>(nullable: false)
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
                        name: "FK_WritePermissions_Identity_SharedByUniqueId",
                        column: x => x.SharedByUniqueId,
                        principalTable: "Identity",
                        principalColumn: "UniqueId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ReadPermissions_SharedByUniqueId",
                table: "ReadPermissions",
                column: "SharedByUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadPermissions_UniqueId",
                table: "ReadPermissions",
                column: "UniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_WritePermissions_SharedByUniqueId",
                table: "WritePermissions",
                column: "SharedByUniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_WritePermissions_UniqueId",
                table: "WritePermissions",
                column: "UniqueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadPermissions");

            migrationBuilder.DropTable(
                name: "WritePermissions");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.AlterColumn<string>(
                name: "Issuer",
                table: "Identity",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Identity",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }
    }
}
