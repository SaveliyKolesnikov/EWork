using Microsoft.EntityFrameworkCore.Migrations;

namespace EWork.Migrations
{
    public partial class AddIsClosedPropToJobModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Jobs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Jobs");
        }
    }
}
