using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "Audit", "DateTime", "FirstName", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "df271f41-d714-4527-8b92-bc8d9d3f6a9c", "باهنر", null, new DateTime(2019, 4, 10, 22, 54, 25, 931, DateTimeKind.Local).AddTicks(8049), "هانی", "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "df271f41-d714-4527-8b92-bc8d9d3f6a9c");
        }
    }
}
