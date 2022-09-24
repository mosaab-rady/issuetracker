using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace issuetracker.Database.Migrations
{
    public partial class priority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_tags_priority_id",
                table: "issues");

            migrationBuilder.DropColumn(
                name: "discriminator",
                table: "tags");

            migrationBuilder.CreateTable(
                name: "priority",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_priority", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_issues_priority_priority_id",
                table: "issues",
                column: "priority_id",
                principalTable: "priority",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_priority_priority_id",
                table: "issues");

            migrationBuilder.DropTable(
                name: "priority");

            migrationBuilder.AddColumn<string>(
                name: "discriminator",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_tags_priority_id",
                table: "issues",
                column: "priority_id",
                principalTable: "tags",
                principalColumn: "id");
        }
    }
}
