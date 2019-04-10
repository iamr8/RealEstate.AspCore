using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class Check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "df271f41-d714-4527-8b92-bc8d9d3f6a9c");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "Audit", "DateTime", "FirstName", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "f3831974-3e21-4c05-9f62-554b0114c641", "باهنر", null, new DateTime(2019, 4, 10, 23, 3, 52, 378, DateTimeKind.Local).AddTicks(9582), "هانی", "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "f3831974-3e21-4c05-9f62-554b0114c641");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "Audit", "DateTime", "FirstName", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "df271f41-d714-4527-8b92-bc8d9d3f6a9c", "باهنر", null, new DateTime(2019, 4, 10, 22, 54, 25, 931, DateTimeKind.Local).AddTicks(8049), "هانی", "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });
        }
    }
}
