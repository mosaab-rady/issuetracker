using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace issuetracker.Database.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    target_end_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    actual_end_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    title = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    target_resolution_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    actual_resolution_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    priority_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resoliotion_summary = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issues", x => x.id);
                    table.ForeignKey(
                        name: "fk_issues_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    discriminator = table.Column<string>(type: "text", nullable: false),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_tags_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_issues_priority_id",
                table: "issues",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_project_id",
                table: "issues",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_name",
                table: "projects",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_issue_id",
                table: "tags",
                column: "issue_id");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_tags_priority_id",
                table: "issues",
                column: "priority_id",
                principalTable: "tags",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_projects_project_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "fk_issues_tags_priority_id",
                table: "issues");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "issues");
        }
    }
}
