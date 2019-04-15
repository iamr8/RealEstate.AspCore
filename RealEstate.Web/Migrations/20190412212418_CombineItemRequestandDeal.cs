using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class CombineItemRequestandDeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicant_ItemRequest_ItemRequestId",
                table: "Applicant");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_ItemRequest_ItemRequestId",
                table: "Deal");

            migrationBuilder.DropTable(
                name: "ItemRequest");

            migrationBuilder.DropIndex(
                name: "IX_Deal_ItemRequestId",
                table: "Deal");

            migrationBuilder.DropColumn(
                name: "ItemRequestId",
                table: "Deal");

            migrationBuilder.RenameColumn(
                name: "ItemRequestId",
                table: "Applicant",
                newName: "DealId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicant_ItemRequestId",
                table: "Applicant",
                newName: "IX_Applicant_DealId");

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "Deal",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Deal",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Deal_ItemId",
                table: "Deal",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicant_Deal_DealId",
                table: "Applicant",
                column: "DealId",
                principalTable: "Deal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_Item_ItemId",
                table: "Deal",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicant_Deal_DealId",
                table: "Applicant");

            migrationBuilder.DropForeignKey(
                name: "FK_Deal_Item_ItemId",
                table: "Deal");

            migrationBuilder.DropIndex(
                name: "IX_Deal_ItemId",
                table: "Deal");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Deal");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Deal");

            migrationBuilder.RenameColumn(
                name: "DealId",
                table: "Applicant",
                newName: "ItemRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Applicant_DealId",
                table: "Applicant",
                newName: "IX_Applicant_ItemRequestId");

            migrationBuilder.AddColumn<string>(
                name: "ItemRequestId",
                table: "Deal",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemRequest",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(nullable: true),
                    IsReject = table.Column<bool>(nullable: false),
                    ItemId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRequest_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_ItemRequestId",
                table: "Deal",
                column: "ItemRequestId",
                unique: true,
                filter: "[ItemRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequest_ItemId",
                table: "ItemRequest",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicant_ItemRequest_ItemRequestId",
                table: "Applicant",
                column: "ItemRequestId",
                principalTable: "ItemRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deal_ItemRequest_ItemRequestId",
                table: "Deal",
                column: "ItemRequestId",
                principalTable: "ItemRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
