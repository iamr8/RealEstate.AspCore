using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class DbUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ownership_Property_PropertyId",
                table: "Ownership");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "Ownership",
                newName: "ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Ownership_PropertyId",
                table: "Ownership",
                newName: "IX_Ownership_ContactId");

            migrationBuilder.AddColumn<int>(
                name: "Dong",
                table: "Ownership",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PropertyOwnershipId",
                table: "Ownership",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PropertyOwnership",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    PropertyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyOwnership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyOwnership_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ownership_PropertyOwnershipId",
                table: "Ownership",
                column: "PropertyOwnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOwnership_PropertyId",
                table: "PropertyOwnership",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ownership_Contact_ContactId",
                table: "Ownership",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ownership_PropertyOwnership_PropertyOwnershipId",
                table: "Ownership",
                column: "PropertyOwnershipId",
                principalTable: "PropertyOwnership",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ownership_Contact_ContactId",
                table: "Ownership");

            migrationBuilder.DropForeignKey(
                name: "FK_Ownership_PropertyOwnership_PropertyOwnershipId",
                table: "Ownership");

            migrationBuilder.DropTable(
                name: "PropertyOwnership");

            migrationBuilder.DropIndex(
                name: "IX_Ownership_PropertyOwnershipId",
                table: "Ownership");

            migrationBuilder.DropColumn(
                name: "Dong",
                table: "Ownership");

            migrationBuilder.DropColumn(
                name: "PropertyOwnershipId",
                table: "Ownership");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "Ownership",
                newName: "PropertyId");

            migrationBuilder.RenameIndex(
                name: "IX_Ownership_ContactId",
                table: "Ownership",
                newName: "IX_Ownership_PropertyId");

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ContactId = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Dong = table.Column<int>(nullable: false),
                    OwnershipId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Owner_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Owner_Ownership_OwnershipId",
                        column: x => x.OwnershipId,
                        principalTable: "Ownership",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Owner_ContactId",
                table: "Owner",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_OwnershipId",
                table: "Owner",
                column: "OwnershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ownership_Property_PropertyId",
                table: "Ownership",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
