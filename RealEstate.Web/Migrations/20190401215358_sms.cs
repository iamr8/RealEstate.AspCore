using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class sms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemRequest_Deal_DealId",
                table: "ItemRequest");

            migrationBuilder.DropIndex(
                name: "IX_ItemRequest_DealId",
                table: "ItemRequest");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "ed50fdad-ad1e-4db3-bd27-3eeb2aed20ec");

            migrationBuilder.DropColumn(
                name: "DealId",
                table: "ItemRequest");

            migrationBuilder.AddColumn<string>(
                name: "SmsId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsTemplateId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemRequestId",
                table: "Deal",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmsTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Sender = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    ReferenceId = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Provider = table.Column<int>(nullable: false),
                    StatusJson = table.Column<string>(nullable: true),
                    ContactId = table.Column<string>(nullable: true),
                    SmsTemplateId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sms_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sms_SmsTemplate_SmsTemplateId",
                        column: x => x.SmsTemplateId,
                        principalTable: "SmsTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sms_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "DateOfPay", "DateTime", "FirstName", "FixedSalary", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "1c4f31ce-b765-4a36-acba-722db789112e", "باهنر", new DateTime(2019, 4, 2, 2, 23, 57, 794, DateTimeKind.Local).AddTicks(3590), new DateTime(2019, 4, 2, 2, 23, 57, 756, DateTimeKind.Local).AddTicks(9340), "هانی", 3600000.0, "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Log_SmsId",
                table: "Log",
                column: "SmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_SmsTemplateId",
                table: "Log",
                column: "SmsTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_ItemRequestId",
                table: "Deal",
                column: "ItemRequestId",
                unique: true,
                filter: "[ItemRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sms_ContactId",
                table: "Sms",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Sms_SmsTemplateId",
                table: "Sms",
                column: "SmsTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Sms_UserId",
                table: "Sms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_ItemRequest_ItemRequestId",
                table: "Deal",
                column: "ItemRequestId",
                principalTable: "ItemRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Sms_SmsId",
                table: "Log",
                column: "SmsId",
                principalTable: "Sms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_SmsTemplate_SmsTemplateId",
                table: "Log",
                column: "SmsTemplateId",
                principalTable: "SmsTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deal_ItemRequest_ItemRequestId",
                table: "Deal");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Sms_SmsId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_SmsTemplate_SmsTemplateId",
                table: "Log");

            migrationBuilder.DropTable(
                name: "Sms");

            migrationBuilder.DropTable(
                name: "SmsTemplate");

            migrationBuilder.DropIndex(
                name: "IX_Log_SmsId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_SmsTemplateId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Deal_ItemRequestId",
                table: "Deal");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "1c4f31ce-b765-4a36-acba-722db789112e");

            migrationBuilder.DropColumn(
                name: "SmsId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "SmsTemplateId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ItemRequestId",
                table: "Deal");

            migrationBuilder.AddColumn<string>(
                name: "DealId",
                table: "ItemRequest",
                nullable: true);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "DateOfPay", "DateTime", "FirstName", "FixedSalary", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "ed50fdad-ad1e-4db3-bd27-3eeb2aed20ec", "باهنر", new DateTime(2019, 3, 31, 20, 59, 59, 138, DateTimeKind.Local).AddTicks(3552), new DateTime(2019, 3, 31, 20, 59, 59, 109, DateTimeKind.Local).AddTicks(7395), "هانی", 3600000.0, "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequest_DealId",
                table: "ItemRequest",
                column: "DealId",
                unique: true,
                filter: "[DealId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemRequest_Deal_DealId",
                table: "ItemRequest",
                column: "DealId",
                principalTable: "Deal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
