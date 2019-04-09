using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace RealEstate.Web.Migrations
{
    public partial class AddReminderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReminderId",
                table: "Log",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reminder",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UserId = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    AlarmTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminder_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Log_ReminderId",
                table: "Log",
                column: "ReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminder_UserId",
                table: "Reminder",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Reminder_ReminderId",
                table: "Log",
                column: "ReminderId",
                principalTable: "Reminder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_Reminder_ReminderId",
                table: "Log");

            migrationBuilder.DropTable(
                name: "Reminder");

            migrationBuilder.DropIndex(
                name: "IX_Log_ReminderId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ReminderId",
                table: "Log");
        }
    }
}