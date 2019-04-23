using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class UpdateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "0e9aa361-67db-4ff2-9406-f619964d183b");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "1812466e-880a-4f25-97a0-6ad728fc99a6");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "26fb3d49-2063-44e2-b537-d36ad1e99177");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "567d7eff-8a0c-4884-902e-3a12066bf65a");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "611272f2-ad7b-42e2-ba1c-d5a28500a230");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "93ca1b9d-56d2-4519-b076-ebfeb1a55067");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "c6992436-f6c6-4a2f-b57e-2fb81ac3ed44");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "ec4072f8-17c0-4b58-b612-e516640c9815");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "01767e17-ea8e-4723-a7fb-629b1f8c205d");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "1da89a67-308a-4d48-96da-bc90f243e837");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "6766f4b7-df3b-4ab6-a1c5-0c5268d1d727");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "8c89e1fd-e3d3-4ff8-bac5-df33ebade352");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "d4530f24-618c-4dac-a331-eabfa0912be6");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "e902b808-5285-4e58-b5ac-fb9e2f711d06");

            migrationBuilder.DeleteData(
                table: "Division",
                keyColumn: "Id",
                keyValue: "4c373207-e9b4-41f6-a6e2-f78c4e69039b");

            migrationBuilder.DeleteData(
                table: "EmployeeDivision",
                keyColumn: "Id",
                keyValue: "037fe9ea-f8e2-4628-8b30-9a78298a1107");

            migrationBuilder.DeleteData(
                table: "EmployeeStatus",
                keyColumn: "Id",
                keyValue: "e5a9414f-5468-4f01-b845-501f07bb7a09");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "2e08b9ee-f9ae-4e5e-9f62-c4e234119b2c");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "4ec5b0c5-09e7-42bc-8112-f737d7e23cc5");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "67eb9a22-a082-48df-96ef-26acd55578b7");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "8861edff-f54a-4c16-90f2-05e546426a58");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "cd1e4690-31f8-45ab-ae01-416caca98de8");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "254b724f-47da-43b8-a610-19c366ba7bd4");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "71e8168a-996f-4441-9c6e-2bc67596e760");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "988b025f-a470-4abf-b417-cbdc9f7be6f0");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "aea4ba65-59ca-4047-ab51-287f2cdcd548");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "b20e5e79-e855-49cd-8621-0cb2699783aa");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "e740fd67-f2d8-4155-9d14-6b1c73962286");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "fe1ffe43-8b9d-4400-9297-f368ffa4602a");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "feefe3c2-a9b3-4c98-b530-6ebbb9d4a924");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "3cb11853-33dc-44b9-a2bc-6026bbbc9887");

            migrationBuilder.DeleteData(
                table: "Division",
                keyColumn: "Id",
                keyValue: "60d60d1e-b9c6-4eac-9388-2342de5e83f6");

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: "9a20d296-12f6-4f07-b00c-5a0ec3b2db61");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Reminder");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "Presence");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Presence");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Presence");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Insurance");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Insurance");

            migrationBuilder.RenameColumn(
                name: "AlarmTime",
                table: "Reminder",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reminder",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Presence",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Insurance",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Employee",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Employee",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Check",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bank",
                table: "Check",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reminder");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Presence");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Reminder",
                newName: "AlarmTime");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Reminder",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Presence",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Presence",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Presence",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Insurance",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Insurance",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Insurance",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Employee",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Employee",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Check",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Bank",
                table: "Check",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "c6992436-f6c6-4a2f-b57e-2fb81ac3ed44", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "خرید و فروش", 0 },
                    { "ec4072f8-17c0-4b58-b612-e516640c9815", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "زمین", 1 },
                    { "567d7eff-8a0c-4884-902e-3a12066bf65a", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "ویلایی", 1 },
                    { "0e9aa361-67db-4ff2-9406-f619964d183b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "مشارکت در ساخت", 0 },
                    { "26fb3d49-2063-44e2-b537-d36ad1e99177", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "رهن و اجاره", 0 },
                    { "611272f2-ad7b-42e2-ba1c-d5a28500a230", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "رهن کامل", 0 },
                    { "93ca1b9d-56d2-4519-b076-ebfeb1a55067", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "آپارتمان", 1 },
                    { "1812466e-880a-4f25-97a0-6ad728fc99a6", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "مغازه", 1 }
                });

            migrationBuilder.InsertData(
                table: "District",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "8c89e1fd-e3d3-4ff8-bac5-df33ebade352", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "زیتون کارمندی" },
                    { "e902b808-5285-4e58-b5ac-fb9e2f711d06", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "زیتون کارگری" },
                    { "1da89a67-308a-4d48-96da-bc90f243e837", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "باهنر" },
                    { "6766f4b7-df3b-4ab6-a1c5-0c5268d1d727", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "کیان آباد" },
                    { "d4530f24-618c-4dac-a331-eabfa0912be6", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "کیانپارس" },
                    { "01767e17-ea8e-4723-a7fb-629b1f8c205d", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "ملیراه" }
                });

            migrationBuilder.InsertData(
                table: "Division",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "4c373207-e9b4-41f6-a6e2-f78c4e69039b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "کارواش" },
                    { "60d60d1e-b9c6-4eac-9388-2342de5e83f6", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "املاک" }
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "Address", "Audit", "FirstName", "LastName", "Mobile", "Phone" },
                values: new object[] { "9a20d296-12f6-4f07-b00c-5a0ec3b2db61", "باهنر", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "هانی", "موسی زاده", "09166000341", "33379367" });

            migrationBuilder.InsertData(
                table: "Facility",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "cd1e4690-31f8-45ab-ae01-416caca98de8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "سالن بدنسازی" },
                    { "8861edff-f54a-4c16-90f2-05e546426a58", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "پارکینگ" },
                    { "2e08b9ee-f9ae-4e5e-9f62-c4e234119b2c", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "آسانسور" },
                    { "67eb9a22-a082-48df-96ef-26acd55578b7", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "سالن همایش" },
                    { "4ec5b0c5-09e7-42bc-8112-f737d7e23cc5", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "آنتن مرکزی" }
                });

            migrationBuilder.InsertData(
                table: "Feature",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "254b724f-47da-43b8-a610-19c366ba7bd4", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "بر زمین", 1 },
                    { "e740fd67-f2d8-4155-9d14-6b1c73962286", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "قیمت نهایی", 0 },
                    { "feefe3c2-a9b3-4c98-b530-6ebbb9d4a924", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "بودجه", 2 },
                    { "988b025f-a470-4abf-b417-cbdc9f7be6f0", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "متراژ", 1 },
                    { "aea4ba65-59ca-4047-ab51-287f2cdcd548", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "پیش پرداخت", 2 },
                    { "b20e5e79-e855-49cd-8621-0cb2699783aa", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "قیمت هر متر", 0 },
                    { "71e8168a-996f-4441-9c6e-2bc67596e760", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "تعداد خواب", 1 },
                    { "fe1ffe43-8b9d-4400-9297-f368ffa4602a", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "کرایه", 2 }
                });

            migrationBuilder.InsertData(
                table: "EmployeeDivision",
                columns: new[] { "Id", "Audit", "DivisionId", "EmployeeId" },
                values: new object[] { "037fe9ea-f8e2-4628-8b30-9a78298a1107", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "60d60d1e-b9c6-4eac-9388-2342de5e83f6", "9a20d296-12f6-4f07-b00c-5a0ec3b2db61" });

            migrationBuilder.InsertData(
                table: "EmployeeStatus",
                columns: new[] { "Id", "Audit", "EmployeeId", "Status" },
                values: new object[] { "e5a9414f-5468-4f01-b845-501f07bb7a09", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "9a20d296-12f6-4f07-b00c-5a0ec3b2db61", 0 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Audit", "EmployeeId", "Password", "Role", "Username" },
                values: new object[] { "3cb11853-33dc-44b9-a2bc-6026bbbc9887", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-22T22:47:17.5773362+04:30\",\"t\":0}]", "9a20d296-12f6-4f07-b00c-5a0ec3b2db61", "YmAdyc6Ph9PNcJOLeira6w==", 2, "admin" });
        }
    }
}
