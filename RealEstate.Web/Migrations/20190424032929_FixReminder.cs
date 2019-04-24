using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class FixReminder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Reminder",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Reminder",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
