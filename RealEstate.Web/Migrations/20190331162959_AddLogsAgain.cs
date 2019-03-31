using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class AddLogsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "21e5c28f-0221-4669-b535-8bca88ee647e");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Log");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantFeatureId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicantId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DealPaymentId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacilityId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemCategoryId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemFeatureId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemRequestId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyCategoryId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyFacilityId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyFeatureId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyOwnershipId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserItemCategoryId",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPropertyCategoryId",
                table: "Log",
                nullable: true);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "DateOfPay", "DateTime", "FirstName", "FixedSalary", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "ed50fdad-ad1e-4db3-bd27-3eeb2aed20ec", "باهنر", new DateTime(2019, 3, 31, 20, 59, 59, 138, DateTimeKind.Local).AddTicks(3552), new DateTime(2019, 3, 31, 20, 59, 59, 109, DateTimeKind.Local).AddTicks(7395), "هانی", 3600000.0, "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Log_ApplicantFeatureId",
                table: "Log",
                column: "ApplicantFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_ApplicantId",
                table: "Log",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_BeneficiaryId",
                table: "Log",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_ContactId",
                table: "Log",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_DealId",
                table: "Log",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_DealPaymentId",
                table: "Log",
                column: "DealPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_DistrictId",
                table: "Log",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_FacilityId",
                table: "Log",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_FeatureId",
                table: "Log",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_ItemCategoryId",
                table: "Log",
                column: "ItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_ItemFeatureId",
                table: "Log",
                column: "ItemFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_ItemId",
                table: "Log",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_ItemRequestId",
                table: "Log",
                column: "ItemRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_LogId",
                table: "Log",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_OwnershipId",
                table: "Log",
                column: "OwnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PaymentId",
                table: "Log",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PictureId",
                table: "Log",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PropertyCategoryId",
                table: "Log",
                column: "PropertyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PropertyFacilityId",
                table: "Log",
                column: "PropertyFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PropertyFeatureId",
                table: "Log",
                column: "PropertyFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PropertyId",
                table: "Log",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PropertyOwnershipId",
                table: "Log",
                column: "PropertyOwnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserItemCategoryId",
                table: "Log",
                column: "UserItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserPropertyCategoryId",
                table: "Log",
                column: "UserPropertyCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_ApplicantFeature_ApplicantFeatureId",
                table: "Log",
                column: "ApplicantFeatureId",
                principalTable: "ApplicantFeature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Applicant_ApplicantId",
                table: "Log",
                column: "ApplicantId",
                principalTable: "Applicant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Beneficiary_BeneficiaryId",
                table: "Log",
                column: "BeneficiaryId",
                principalTable: "Beneficiary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Contact_ContactId",
                table: "Log",
                column: "ContactId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Deal_DealId",
                table: "Log",
                column: "DealId",
                principalTable: "Deal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_DealPayment_DealPaymentId",
                table: "Log",
                column: "DealPaymentId",
                principalTable: "DealPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_District_DistrictId",
                table: "Log",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Facility_FacilityId",
                table: "Log",
                column: "FacilityId",
                principalTable: "Facility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Feature_FeatureId",
                table: "Log",
                column: "FeatureId",
                principalTable: "Feature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_ItemCategory_ItemCategoryId",
                table: "Log",
                column: "ItemCategoryId",
                principalTable: "ItemCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_ItemFeature_ItemFeatureId",
                table: "Log",
                column: "ItemFeatureId",
                principalTable: "ItemFeature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Item_ItemId",
                table: "Log",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_ItemRequest_ItemRequestId",
                table: "Log",
                column: "ItemRequestId",
                principalTable: "ItemRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Log_LogId",
                table: "Log",
                column: "LogId",
                principalTable: "Log",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Ownership_OwnershipId",
                table: "Log",
                column: "OwnershipId",
                principalTable: "Ownership",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Payment_PaymentId",
                table: "Log",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Picture_PictureId",
                table: "Log",
                column: "PictureId",
                principalTable: "Picture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_PropertyCategory_PropertyCategoryId",
                table: "Log",
                column: "PropertyCategoryId",
                principalTable: "PropertyCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_PropertyFacility_PropertyFacilityId",
                table: "Log",
                column: "PropertyFacilityId",
                principalTable: "PropertyFacility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_PropertyFeature_PropertyFeatureId",
                table: "Log",
                column: "PropertyFeatureId",
                principalTable: "PropertyFeature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Property_PropertyId",
                table: "Log",
                column: "PropertyId",
                principalTable: "Property",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_PropertyOwnership_PropertyOwnershipId",
                table: "Log",
                column: "PropertyOwnershipId",
                principalTable: "PropertyOwnership",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_User_UserId",
                table: "Log",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_UserItemCategory_UserItemCategoryId",
                table: "Log",
                column: "UserItemCategoryId",
                principalTable: "UserItemCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Log_UserPropertyCategory_UserPropertyCategoryId",
                table: "Log",
                column: "UserPropertyCategoryId",
                principalTable: "UserPropertyCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_ApplicantFeature_ApplicantFeatureId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Applicant_ApplicantId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Beneficiary_BeneficiaryId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Contact_ContactId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Deal_DealId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_DealPayment_DealPaymentId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_District_DistrictId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Facility_FacilityId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Feature_FeatureId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_ItemCategory_ItemCategoryId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_ItemFeature_ItemFeatureId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Item_ItemId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_ItemRequest_ItemRequestId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Log_LogId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Ownership_OwnershipId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Payment_PaymentId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Picture_PictureId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_PropertyCategory_PropertyCategoryId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_PropertyFacility_PropertyFacilityId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_PropertyFeature_PropertyFeatureId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_Property_PropertyId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_PropertyOwnership_PropertyOwnershipId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_User_UserId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_UserItemCategory_UserItemCategoryId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Log_UserPropertyCategory_UserPropertyCategoryId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ApplicantFeatureId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ApplicantId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_BeneficiaryId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ContactId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_DealId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_DealPaymentId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_DistrictId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_FacilityId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_FeatureId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ItemCategoryId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ItemFeatureId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ItemId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_ItemRequestId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_LogId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_OwnershipId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PaymentId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PictureId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PropertyCategoryId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PropertyFacilityId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PropertyFeatureId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PropertyId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_PropertyOwnershipId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_UserId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_UserItemCategoryId",
                table: "Log");

            migrationBuilder.DropIndex(
                name: "IX_Log_UserPropertyCategoryId",
                table: "Log");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: "ed50fdad-ad1e-4db3-bd27-3eeb2aed20ec");

            migrationBuilder.DropColumn(
                name: "ApplicantFeatureId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "BeneficiaryId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "DealId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "DealPaymentId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "FeatureId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ItemCategoryId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ItemFeatureId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ItemRequestId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "LogId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "OwnershipId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PropertyCategoryId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PropertyFacilityId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PropertyFeatureId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PropertyOwnershipId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "UserItemCategoryId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "UserPropertyCategoryId",
                table: "Log");

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "Log",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "DateOfPay", "DateTime", "FirstName", "FixedSalary", "LastName", "Mobile", "Password", "Phone", "Role", "Username" },
                values: new object[] { "21e5c28f-0221-4669-b535-8bca88ee647e", "باهنر", new DateTime(2019, 3, 31, 1, 16, 23, 762, DateTimeKind.Local).AddTicks(6163), new DateTime(2019, 3, 31, 1, 16, 23, 735, DateTimeKind.Local).AddTicks(8935), "هانی", 3600000.0, "موسی زاده", "09166000341", "YmAdyc6Ph9PNcJOLeira6w==", "33379367", 2, "admin" });
        }
    }
}
