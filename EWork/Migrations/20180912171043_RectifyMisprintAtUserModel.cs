using Microsoft.EntityFrameworkCore.Migrations;

namespace EWork.Migrations
{
    public partial class RectifyMisprintAtUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SingUpDate",
                table: "AspNetUsers",
                newName: "SignUpDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignUpDate",
                table: "AspNetUsers",
                newName: "SingUpDate");
        }
    }
}
