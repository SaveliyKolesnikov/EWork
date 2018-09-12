using Microsoft.EntityFrameworkCore.Migrations;

namespace EWork.Migrations
{
    public partial class addAdministratorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorId",
                table: "Jobs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AdministratorId",
                table: "Jobs",
                column: "AdministratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_AdministratorId",
                table: "Jobs",
                column: "AdministratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_AdministratorId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_AdministratorId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "AdministratorId",
                table: "Jobs");
        }
    }
}
