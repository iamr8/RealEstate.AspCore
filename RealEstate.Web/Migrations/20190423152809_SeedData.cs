using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "8b8c5e5b-2baf-4132-9d15-6d9393ef3504", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "خرید و فروش", 0 },
                    { "b0adb618-233f-44d3-b2a3-9bb32c6111cd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "زمین", 1 },
                    { "460a79cd-a5be-4dea-957c-ba0ab972acdd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "ویلایی", 1 },
                    { "279532ad-5486-4177-baae-c672020679bd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "مشارکت در ساخت", 0 },
                    { "5e4de8fc-2d15-4e3a-bede-f10906c6e7c8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "رهن و اجاره", 0 },
                    { "cd290ddd-23f9-4cf0-855e-f0b0c66cf1d9", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "رهن کامل", 0 },
                    { "99b4c960-b944-460d-9e8b-cdc226e15adf", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "آپارتمان", 1 },
                    { "48a1f35f-74c3-4580-812b-26662e43bb88", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "مغازه", 1 }
                });

            migrationBuilder.InsertData(
                table: "District",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "fd0bab80-829d-41cd-a915-10937d980e1d", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "زیتون کارمندی" },
                    { "47977aa3-ced7-43af-8f98-ed60453ee344", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "زیتون کارگری" },
                    { "5f5be03c-740c-48e1-bb71-37eda18e19d1", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "باهنر" },
                    { "e74a124f-da07-4fdd-8883-0e24497d0a81", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "کیان آباد" },
                    { "d42e0bd1-3702-4276-85f9-8b7f90e91719", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "کیانپارس" },
                    { "28104595-c4dc-4701-94b8-914a51f9e421", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "ملیراه" }
                });

            migrationBuilder.InsertData(
                table: "Division",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "33737ac3-7805-4f9a-b467-515419c1ed87", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "کارواش" },
                    { "1eef928d-1d36-4726-a507-94c9fb462b74", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "املاک" }
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "Address", "Audit", "FirstName", "LastName", "Mobile", "Phone" },
                values: new object[] { "45920ed3-e213-4b96-bb40-992bb5336f03", "باهنر", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "هانی", "موسی زاده", "09166000341", "33379367" });

            migrationBuilder.InsertData(
                table: "Facility",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "6629f102-73fe-4c5e-a83f-8c969b1854b9", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "سالن بدنسازی" },
                    { "1c36a5da-275a-4855-b8f4-daba733c81eb", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "پارکینگ" },
                    { "93d1c568-a2a8-48f1-b74a-650a8fc4b0a1", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "آسانسور" },
                    { "d501d5b7-6fd8-4748-9263-5b909a58852f", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "سالن همایش" },
                    { "0394320b-eec4-4264-b73a-003010cf9f49", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "آنتن مرکزی" }
                });

            migrationBuilder.InsertData(
                table: "Feature",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "dadae5be-a34c-458d-aab3-a397b20f0d14", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "بر زمین", 1 },
                    { "7b3c8c29-fb40-4673-bb94-af4055608b76", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "قیمت نهایی", 0 },
                    { "5687dfb0-28d4-4c55-afe4-632987f3f45e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "بودجه", 2 },
                    { "9e7f8b8f-bc95-4506-ab0f-bbbda493f3cf", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "متراژ", 1 },
                    { "e5426489-51ac-473a-9e8d-27945a61fba5", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "پیش پرداخت", 2 },
                    { "6feb5af4-61fd-4809-b9a1-9b1f70df7f86", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "قیمت هر متر", 0 },
                    { "a200286b-1812-41c6-b43a-39d4d275821b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "تعداد خواب", 1 },
                    { "f08df191-358d-47c1-a481-07de2023f2e0", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "کرایه", 2 }
                });

            migrationBuilder.InsertData(
                table: "EmployeeDivision",
                columns: new[] { "Id", "Audit", "DivisionId", "EmployeeId" },
                values: new object[] { "65955d68-c0fe-4443-87a3-68ca29c2d565", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "1eef928d-1d36-4726-a507-94c9fb462b74", "45920ed3-e213-4b96-bb40-992bb5336f03" });

            migrationBuilder.InsertData(
                table: "EmployeeStatus",
                columns: new[] { "Id", "Audit", "EmployeeId", "Status" },
                values: new object[] { "2a39e8de-6688-49d8-bde5-16e14dfccd6a", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "45920ed3-e213-4b96-bb40-992bb5336f03", 0 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Audit", "EmployeeId", "Password", "Role", "Username" },
                values: new object[] { "0a658015-ea5d-481e-aa14-3c39b0e6d328", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-23T19:58:07.7504351+04:30\",\"t\":0}]", "45920ed3-e213-4b96-bb40-992bb5336f03", "YmAdyc6Ph9PNcJOLeira6w==", 2, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "279532ad-5486-4177-baae-c672020679bd");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "460a79cd-a5be-4dea-957c-ba0ab972acdd");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "48a1f35f-74c3-4580-812b-26662e43bb88");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "5e4de8fc-2d15-4e3a-bede-f10906c6e7c8");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "8b8c5e5b-2baf-4132-9d15-6d9393ef3504");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "99b4c960-b944-460d-9e8b-cdc226e15adf");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "b0adb618-233f-44d3-b2a3-9bb32c6111cd");

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: "cd290ddd-23f9-4cf0-855e-f0b0c66cf1d9");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "28104595-c4dc-4701-94b8-914a51f9e421");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "47977aa3-ced7-43af-8f98-ed60453ee344");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "5f5be03c-740c-48e1-bb71-37eda18e19d1");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "d42e0bd1-3702-4276-85f9-8b7f90e91719");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "e74a124f-da07-4fdd-8883-0e24497d0a81");

            migrationBuilder.DeleteData(
                table: "District",
                keyColumn: "Id",
                keyValue: "fd0bab80-829d-41cd-a915-10937d980e1d");

            migrationBuilder.DeleteData(
                table: "Division",
                keyColumn: "Id",
                keyValue: "33737ac3-7805-4f9a-b467-515419c1ed87");

            migrationBuilder.DeleteData(
                table: "EmployeeDivision",
                keyColumn: "Id",
                keyValue: "65955d68-c0fe-4443-87a3-68ca29c2d565");

            migrationBuilder.DeleteData(
                table: "EmployeeStatus",
                keyColumn: "Id",
                keyValue: "2a39e8de-6688-49d8-bde5-16e14dfccd6a");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "0394320b-eec4-4264-b73a-003010cf9f49");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "1c36a5da-275a-4855-b8f4-daba733c81eb");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "6629f102-73fe-4c5e-a83f-8c969b1854b9");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "93d1c568-a2a8-48f1-b74a-650a8fc4b0a1");

            migrationBuilder.DeleteData(
                table: "Facility",
                keyColumn: "Id",
                keyValue: "d501d5b7-6fd8-4748-9263-5b909a58852f");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "5687dfb0-28d4-4c55-afe4-632987f3f45e");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "6feb5af4-61fd-4809-b9a1-9b1f70df7f86");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "7b3c8c29-fb40-4673-bb94-af4055608b76");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "9e7f8b8f-bc95-4506-ab0f-bbbda493f3cf");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "a200286b-1812-41c6-b43a-39d4d275821b");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "dadae5be-a34c-458d-aab3-a397b20f0d14");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "e5426489-51ac-473a-9e8d-27945a61fba5");

            migrationBuilder.DeleteData(
                table: "Feature",
                keyColumn: "Id",
                keyValue: "f08df191-358d-47c1-a481-07de2023f2e0");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "0a658015-ea5d-481e-aa14-3c39b0e6d328");

            migrationBuilder.DeleteData(
                table: "Division",
                keyColumn: "Id",
                keyValue: "1eef928d-1d36-4726-a507-94c9fb462b74");

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: "45920ed3-e213-4b96-bb40-992bb5336f03");
        }
    }
}
