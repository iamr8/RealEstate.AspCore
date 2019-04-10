using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class Check2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "f3831974-3e21-4c05-9f62-554b0114c641");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "Audit", "DateTime", "FirstName", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "63de9bc0-3285-4ba9-9374-6518389b25ec", "باهنر", "[{\"i\":null,\"n\":null,\"m\":null,\"d\":\"2019-04-10T23:14:05.2315564+04:30\",\"t\":0}]", new DateTime(2019, 4, 10, 23, 14, 5, 200, DateTimeKind.Local).AddTicks(2037), "هانی", "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "63de9bc0-3285-4ba9-9374-6518389b25ec");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "Audit", "DateTime", "FirstName", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "f3831974-3e21-4c05-9f62-554b0114c641", "باهنر", null, new DateTime(2019, 4, 10, 23, 3, 52, 378, DateTimeKind.Local).AddTicks(9582), "هانی", "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });
        }
    }
}
