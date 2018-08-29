using Microsoft.EntityFrameworkCore.Migrations;

namespace EWork.Migrations
{
    public partial class ReanmeProfilePhotoProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePhotoUrl",
                table: "AspNetUsers",
                newName: "ProfilePhotoName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePhotoName",
                table: "AspNetUsers",
                newName: "ProfilePhotoUrl");
        }
    }
}
