using Microsoft.EntityFrameworkCore.Migrations;

namespace EWork.Migrations
{
    public partial class addmoderatordbset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModeratorId",
                table: "Jobs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ModeratorId",
                table: "Jobs",
                column: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_ModeratorId",
                table: "Jobs",
                column: "ModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_ModeratorId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ModeratorId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ModeratorId",
                table: "Jobs");
        }
    }
}
