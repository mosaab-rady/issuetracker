using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace issuetracker.Database.Migrations
{
    public partial class removeSlugFromIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "slug",
                table: "issues");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "issues",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
