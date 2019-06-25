using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace backendProject.Migrations
{
    public partial class update_session : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstLogin",
                table: "Session",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Session",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstLogin",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Session");
        }
    }
}
