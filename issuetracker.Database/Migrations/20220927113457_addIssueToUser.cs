using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace issuetracker.Database.Migrations
{
    public partial class addIssueToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_issues_issue_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_projects_project_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_issues_users_created_by_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "fk_projects_users_created_by_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "ix_projects_created_by_id",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "ix_issues_created_by_id",
                table: "issues");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_issue_id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_project_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "issues");

            migrationBuilder.DropColumn(
                name: "issue_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "project_id",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "projects",
                newName: "created_by");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "issues",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "app_user_issue",
                columns: table => new
                {
                    assigned_issues_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_to_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_user_issue", x => new { x.assigned_issues_id, x.assigned_to_id });
                    table.ForeignKey(
                        name: "fk_app_user_issue_issues_assigned_issues_id",
                        column: x => x.assigned_issues_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_app_user_issue_users_assigned_to_id",
                        column: x => x.assigned_to_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_user_project",
                columns: table => new
                {
                    assigned_projects_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_to_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_user_project", x => new { x.assigned_projects_id, x.assigned_to_id });
                    table.ForeignKey(
                        name: "fk_app_user_project_projects_assigned_projects_id",
                        column: x => x.assigned_projects_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_app_user_project_users_assigned_to_id",
                        column: x => x.assigned_to_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_app_user_issue_assigned_to_id",
                table: "app_user_issue",
                column: "assigned_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_app_user_project_assigned_to_id",
                table: "app_user_project",
                column: "assigned_to_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_user_issue");

            migrationBuilder.DropTable(
                name: "app_user_project");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "issues");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "projects",
                newName: "created_by_id");

            migrationBuilder.AddColumn<string>(
                name: "created_by_id",
                table: "issues",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "issue_id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "project_id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_projects_created_by_id",
                table: "projects",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_created_by_id",
                table: "issues",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_issue_id",
                table: "AspNetUsers",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_project_id",
                table: "AspNetUsers",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_issues_issue_id",
                table: "AspNetUsers",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_projects_project_id",
                table: "AspNetUsers",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_users_created_by_id",
                table: "issues",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_projects_users_created_by_id",
                table: "projects",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
