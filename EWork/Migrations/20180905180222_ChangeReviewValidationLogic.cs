using Microsoft.EntityFrameworkCore.Migrations;

namespace EWork.Migrations
{
    public partial class ChangeReviewValidationLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Reviews",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Reviews",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4096);
        }
    }
}
