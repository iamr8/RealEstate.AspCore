using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class UpdatePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sms_Customer_CustomerId",
                table: "Sms");

            migrationBuilder.DropForeignKey(
                name: "FK_Sms_Employee_EmployeeId",
                table: "Sms");

            migrationBuilder.DropIndex(
                name: "IX_Sms_CustomerId",
                table: "Sms");

            migrationBuilder.DropIndex(
                name: "IX_Sms_EmployeeId",
                table: "Sms");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "07d0fc97-1cf2-4688-aee8-24919f68b6bd");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "18dada49-eb92-4435-9d68-4513226376fa");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "25b18d5b-f861-4b2c-bf82-a4875db8eb26");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "296b08fe-3d69-45d5-b8c7-1b2aa5205a0c");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "7ee3c867-cb10-4668-8f9c-e0b6768aca45");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "9c356476-d411-4e7f-a812-39b59f514bad");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "dd0a18d7-7770-4d8d-bb95-3caf4bed298e");

            //migrationBuilder.DeleteData(
            //    table: "Category",
            //    keyColumn: "Id",
            //    keyValue: "fcdf54c9-2e82-4384-acf3-4be023803576");

            //migrationBuilder.DeleteData(
            //    table: "District",
            //    keyColumn: "Id",
            //    keyValue: "245fc791-8e69-400c-a9fe-b62f7edcd639");

            //migrationBuilder.DeleteData(
            //    table: "District",
            //    keyColumn: "Id",
            //    keyValue: "2c5e2312-a2f9-4828-8335-e4b12f12bd0b");

            //migrationBuilder.DeleteData(
            //    table: "District",
            //    keyColumn: "Id",
            //    keyValue: "36010ce4-8c6e-48c9-a0f1-9a72c321add2");

            //migrationBuilder.DeleteData(
            //    table: "District",
            //    keyColumn: "Id",
            //    keyValue: "9370f570-144f-4726-848c-55998ff05592");

            //migrationBuilder.DeleteData(
            //    table: "District",
            //    keyColumn: "Id",
            //    keyValue: "dbaa9cd6-bff6-43f2-b0a7-0f8626894964");

            //migrationBuilder.DeleteData(
            //    table: "District",
            //    keyColumn: "Id",
            //    keyValue: "f65efe6f-d7e5-468b-b824-6d5b90c146f2");

            //migrationBuilder.DeleteData(
            //    table: "Division",
            //    keyColumn: "Id",
            //    keyValue: "d196f024-32c7-4c7a-8e7b-f0ba53356583");

            //migrationBuilder.DeleteData(
            //    table: "EmployeeDivision",
            //    keyColumn: "Id",
            //    keyValue: "6a7e9e40-3535-465a-883f-86fa21cb58d7");

            //migrationBuilder.DeleteData(
            //    table: "EmployeeStatus",
            //    keyColumn: "Id",
            //    keyValue: "77cf3bf9-013f-46e9-8889-7b615e8a8c82");

            //migrationBuilder.DeleteData(
            //    table: "Facility",
            //    keyColumn: "Id",
            //    keyValue: "2dd39a45-fda3-4b60-b58f-bda831d3a7fd");

            //migrationBuilder.DeleteData(
            //    table: "Facility",
            //    keyColumn: "Id",
            //    keyValue: "538e01d9-fbfe-4bc9-8e33-a789edf02cbc");

            //migrationBuilder.DeleteData(
            //    table: "Facility",
            //    keyColumn: "Id",
            //    keyValue: "9098ca45-b036-4c98-a41e-f3ff8dc199cd");

            //migrationBuilder.DeleteData(
            //    table: "Facility",
            //    keyColumn: "Id",
            //    keyValue: "bda52f5f-76b6-49f1-a0b8-a5626d7bf62e");

            //migrationBuilder.DeleteData(
            //    table: "Facility",
            //    keyColumn: "Id",
            //    keyValue: "ea761002-3277-491e-bbc5-08d1b7d1d2a7");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "28f4e37a-aeae-4e00-92e8-f2a0a28f8b40");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "32126744-4fed-406d-8a44-e772611270ad");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "8d4576a7-7728-47de-8c45-e348e30e67fb");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "9c34c06c-0989-42b4-930e-db50260bb2f9");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "9eb642bd-5320-4b27-998e-8f90fb95116e");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "aba3615a-c4ab-443d-af8a-a45ccef6d613");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "dde31d12-397e-4a0c-8fd7-d87efd345db6");

            //migrationBuilder.DeleteData(
            //    table: "Feature",
            //    keyColumn: "Id",
            //    keyValue: "f74540f2-140b-4b34-9789-b4823bb940bb");

            //migrationBuilder.DeleteData(
            //    table: "User",
            //    keyColumn: "Id",
            //    keyValue: "85bf213c-2af9-4e36-ae45-4d4f5dfd2144");

            //migrationBuilder.DeleteData(
            //    table: "Division",
            //    keyColumn: "Id",
            //    keyValue: "0eb73618-1fd1-4bd0-9bdf-d1b8203905c8");

            //migrationBuilder.DeleteData(
            //    table: "Employee",
            //    keyColumn: "Id",
            //    keyValue: "e17a64aa-9732-4e11-ba0a-26ee515d8dd5");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Sms");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Sms");

            migrationBuilder.AddColumn<string>(
                name: "CheckoutId",
                table: "Payment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsId",
                table: "Payment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsId",
                table: "DealRequest",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CheckoutId",
                table: "Payment",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_SmsId",
                table: "Payment",
                column: "SmsId");

            migrationBuilder.CreateIndex(
                name: "IX_DealRequest_SmsId",
                table: "DealRequest",
                column: "SmsId");

            migrationBuilder.AddForeignKey(
                name: "FK_DealRequest_Sms_SmsId",
                table: "DealRequest",
                column: "SmsId",
                principalTable: "Sms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Payment_CheckoutId",
                table: "Payment",
                column: "CheckoutId",
                principalTable: "Payment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Sms_SmsId",
                table: "Payment",
                column: "SmsId",
                principalTable: "Sms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DealRequest_Sms_SmsId",
                table: "DealRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Payment_CheckoutId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Sms_SmsId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_CheckoutId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_SmsId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_DealRequest_SmsId",
                table: "DealRequest");

            migrationBuilder.DropColumn(
                name: "CheckoutId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "SmsId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "SmsId",
                table: "DealRequest");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Sms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "Sms",
                nullable: true);

            //migrationBuilder.InsertData(
            //    table: "Category",
            //    columns: new[] { "Id", "Audit", "Name", "Type" },
            //    values: new object[,]
            //    {
            //        { "fcdf54c9-2e82-4384-acf3-4be023803576", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "خرید و فروش", 0 },
            //        { "18dada49-eb92-4435-9d68-4513226376fa", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "زمین", 1 },
            //        { "296b08fe-3d69-45d5-b8c7-1b2aa5205a0c", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "ویلایی", 1 },
            //        { "07d0fc97-1cf2-4688-aee8-24919f68b6bd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "مشارکت در ساخت", 0 },
            //        { "7ee3c867-cb10-4668-8f9c-e0b6768aca45", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "رهن و اجاره", 0 },
            //        { "25b18d5b-f861-4b2c-bf82-a4875db8eb26", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "رهن کامل", 0 },
            //        { "9c356476-d411-4e7f-a812-39b59f514bad", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "آپارتمان", 1 },
            //        { "dd0a18d7-7770-4d8d-bb95-3caf4bed298e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "مغازه", 1 }
            //    });

            //migrationBuilder.InsertData(
            //    table: "District",
            //    columns: new[] { "Id", "Audit", "Name" },
            //    values: new object[,]
            //    {
            //        { "dbaa9cd6-bff6-43f2-b0a7-0f8626894964", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "زیتون کارمندی" },
            //        { "f65efe6f-d7e5-468b-b824-6d5b90c146f2", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "زیتون کارگری" },
            //        { "245fc791-8e69-400c-a9fe-b62f7edcd639", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "باهنر" },
            //        { "9370f570-144f-4726-848c-55998ff05592", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کیان آباد" },
            //        { "2c5e2312-a2f9-4828-8335-e4b12f12bd0b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کیانپارس" },
            //        { "36010ce4-8c6e-48c9-a0f1-9a72c321add2", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "ملیراه" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Division",
            //    columns: new[] { "Id", "Audit", "Name" },
            //    values: new object[,]
            //    {
            //        { "d196f024-32c7-4c7a-8e7b-f0ba53356583", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کارواش" },
            //        { "0eb73618-1fd1-4bd0-9bdf-d1b8203905c8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "املاک" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Employee",
            //    columns: new[] { "Id", "Address", "Audit", "FirstName", "LastName", "Mobile", "Phone" },
            //    values: new object[] { "e17a64aa-9732-4e11-ba0a-26ee515d8dd5", "باهنر", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "هانی", "موسی زاده", "09166000341", "33379367" });

            //migrationBuilder.InsertData(
            //    table: "Facility",
            //    columns: new[] { "Id", "Audit", "Name" },
            //    values: new object[,]
            //    {
            //        { "bda52f5f-76b6-49f1-a0b8-a5626d7bf62e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "سالن بدنسازی" },
            //        { "2dd39a45-fda3-4b60-b58f-bda831d3a7fd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "پارکینگ" },
            //        { "ea761002-3277-491e-bbc5-08d1b7d1d2a7", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "آسانسور" },
            //        { "538e01d9-fbfe-4bc9-8e33-a789edf02cbc", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "سالن همایش" },
            //        { "9098ca45-b036-4c98-a41e-f3ff8dc199cd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "آنتن مرکزی" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Feature",
            //    columns: new[] { "Id", "Audit", "Name", "Type" },
            //    values: new object[,]
            //    {
            //        { "9c34c06c-0989-42b4-930e-db50260bb2f9", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "بر زمین", 1 },
            //        { "9eb642bd-5320-4b27-998e-8f90fb95116e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "قیمت نهایی", 0 },
            //        { "8d4576a7-7728-47de-8c45-e348e30e67fb", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "بودجه", 2 },
            //        { "f74540f2-140b-4b34-9789-b4823bb940bb", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "متراژ", 1 },
            //        { "dde31d12-397e-4a0c-8fd7-d87efd345db6", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "پیش پرداخت", 2 },
            //        { "28f4e37a-aeae-4e00-92e8-f2a0a28f8b40", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "قیمت هر متر", 0 },
            //        { "aba3615a-c4ab-443d-af8a-a45ccef6d613", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "تعداد خواب", 1 },
            //        { "32126744-4fed-406d-8a44-e772611270ad", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کرایه", 2 }
            //    });

            //migrationBuilder.InsertData(
            //    table: "EmployeeDivision",
            //    columns: new[] { "Id", "Audit", "DivisionId", "EmployeeId" },
            //    values: new object[] { "6a7e9e40-3535-465a-883f-86fa21cb58d7", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "0eb73618-1fd1-4bd0-9bdf-d1b8203905c8", "e17a64aa-9732-4e11-ba0a-26ee515d8dd5" });

            //migrationBuilder.InsertData(
            //    table: "EmployeeStatus",
            //    columns: new[] { "Id", "Audit", "EmployeeId", "Status" },
            //    values: new object[] { "77cf3bf9-013f-46e9-8889-7b615e8a8c82", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "e17a64aa-9732-4e11-ba0a-26ee515d8dd5", 0 });

            //migrationBuilder.InsertData(
            //    table: "User",
            //    columns: new[] { "Id", "Audit", "EmployeeId", "Password", "Role", "Username" },
            //    values: new object[] { "85bf213c-2af9-4e36-ae45-4d4f5dfd2144", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "e17a64aa-9732-4e11-ba0a-26ee515d8dd5", "YmAdyc6Ph9PNcJOLeira6w==", 2, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Sms_CustomerId",
                table: "Sms",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sms_EmployeeId",
                table: "Sms",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sms_Customer_CustomerId",
                table: "Sms",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sms_Employee_EmployeeId",
                table: "Sms",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
