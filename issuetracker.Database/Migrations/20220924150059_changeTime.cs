using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace issuetracker.Database.Migrations
{
    public partial class changeTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_issues_issue_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_tags_issue_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "issue_id",
                table: "tags");

            migrationBuilder.CreateTable(
                name: "issue_tag",
                columns: table => new
                {
                    issues_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tags_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_tag", x => new { x.issues_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_issue_tag_issues_issues_id",
                        column: x => x.issues_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_tag_tags_tags_id",
                        column: x => x.tags_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_issue_tag_tags_id",
                table: "issue_tag",
                column: "tags_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "issue_tag");

            migrationBuilder.AddColumn<Guid>(
                name: "issue_id",
                table: "tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_issue_id",
                table: "tags",
                column: "issue_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_issues_issue_id",
                table: "tags",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id");
        }
    }
}
