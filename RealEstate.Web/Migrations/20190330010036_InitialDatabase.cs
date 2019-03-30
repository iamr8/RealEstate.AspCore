using System;
using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deal",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_District", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feature",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemCategory",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Type = table.Column<int>(nullable: false),
                    CreatorId = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    EntityId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCategory",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Role = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Mobile = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    FixedSalary = table.Column<double>(nullable: false),
                    DateOfPay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DealPayment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    CommissionPrice = table.Column<decimal>(nullable: false),
                    TipPrice = table.Column<decimal>(nullable: false),
                    PayDate = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    DealId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealPayment_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Street = table.Column<string>(nullable: false),
                    Alley = table.Column<string>(nullable: true),
                    BuildingName = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Floor = table.Column<int>(nullable: false),
                    Flat = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DistrictId = table.Column<string>(nullable: false),
                    PropertyCategoryId = table.Column<string>(nullable: false),
                    Geolocation = table.Column<IPoint>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Property_District_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Property_PropertyCategory_PropertyCategoryId",
                        column: x => x.PropertyCategoryId,
                        principalTable: "PropertyCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiary",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    TipPercent = table.Column<int>(nullable: false),
                    CommissionPercent = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    DealId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiary_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beneficiary_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipient",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    LogId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationRecipient_Log_LogId",
                        column: x => x.LogId,
                        principalTable: "Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipient_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSeener",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    LogId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSeener", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSeener_Log_LogId",
                        column: x => x.LogId,
                        principalTable: "Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationSeener_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Value = table.Column<double>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserItemCategory",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UserId = table.Column<string>(nullable: false),
                    ItemCategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserItemCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserItemCategory_ItemCategory_ItemCategoryId",
                        column: x => x.ItemCategoryId,
                        principalTable: "ItemCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserItemCategory_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPropertyCategory",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UserId = table.Column<string>(nullable: false),
                    PropertyCategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPropertyCategory_PropertyCategory_PropertyCategoryId",
                        column: x => x.PropertyCategoryId,
                        principalTable: "PropertyCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPropertyCategory_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(nullable: true),
                    PropertyId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_ItemCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ItemCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ownership",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    PropertyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ownership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ownership_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFacility",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    PropertyId = table.Column<string>(nullable: false),
                    FacilityId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFacility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyFacility_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyFacility_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFeature",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Value = table.Column<string>(nullable: false),
                    PropertyId = table.Column<string>(nullable: false),
                    FeatureId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyFeature_Feature_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Feature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyFeature_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    File = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    PropertyId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true),
                    DealId = table.Column<string>(nullable: true),
                    DealPaymentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Picture_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Picture_DealPayment_DealPaymentId",
                        column: x => x.DealPaymentId,
                        principalTable: "DealPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Picture_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Picture_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemFeature",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Value = table.Column<string>(nullable: false),
                    ItemId = table.Column<string>(nullable: false),
                    FeatureId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFeature_Feature_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Feature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemFeature_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemRequest",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Description = table.Column<string>(nullable: true),
                    IsReject = table.Column<bool>(nullable: false),
                    ItemId = table.Column<string>(nullable: false),
                    DealId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRequest_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemRequest_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Dong = table.Column<int>(nullable: false),
                    OwnershipId = table.Column<string>(nullable: true),
                    ContactId = table.Column<string>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Applicant",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    ContactId = table.Column<string>(nullable: false),
                    ItemRequestId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicant_Contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applicant_ItemRequest_ItemRequestId",
                        column: x => x.ItemRequestId,
                        principalTable: "ItemRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applicant_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantFeature",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    Value = table.Column<string>(nullable: false),
                    ApplicantId = table.Column<string>(nullable: false),
                    FeatureId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantFeature_Applicant_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantFeature_Feature_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Feature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_ContactId",
                table: "Applicant",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_ItemRequestId",
                table: "Applicant",
                column: "ItemRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_UserId",
                table: "Applicant",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantFeature_ApplicantId",
                table: "ApplicantFeature",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantFeature_FeatureId",
                table: "ApplicantFeature",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiary_DealId",
                table: "Beneficiary",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiary_UserId",
                table: "Beneficiary",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DealPayment_DealId",
                table: "DealPayment",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryId",
                table: "Item",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_PropertyId",
                table: "Item",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFeature_FeatureId",
                table: "ItemFeature",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFeature_ItemId",
                table: "ItemFeature",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequest_DealId",
                table: "ItemRequest",
                column: "DealId",
                unique: true,
                filter: "[DealId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequest_ItemId",
                table: "ItemRequest",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipient_LogId",
                table: "NotificationRecipient",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipient_UserId",
                table: "NotificationRecipient",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeener_LogId",
                table: "NotificationSeener",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeener_UserId",
                table: "NotificationSeener",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_ContactId",
                table: "Owner",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_OwnershipId",
                table: "Owner",
                column: "OwnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Ownership_PropertyId",
                table: "Ownership",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserId",
                table: "Payment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_DealId",
                table: "Picture",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_DealPaymentId",
                table: "Picture",
                column: "DealPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_PaymentId",
                table: "Picture",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_PropertyId",
                table: "Picture",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_DistrictId",
                table: "Property",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_PropertyCategoryId",
                table: "Property",
                column: "PropertyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFacility_FacilityId",
                table: "PropertyFacility",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFacility_PropertyId",
                table: "PropertyFacility",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFeature_FeatureId",
                table: "PropertyFeature",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFeature_PropertyId",
                table: "PropertyFeature",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Mobile",
                table: "User",
                column: "Mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserItemCategory_ItemCategoryId",
                table: "UserItemCategory",
                column: "ItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserItemCategory_UserId",
                table: "UserItemCategory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyCategory_PropertyCategoryId",
                table: "UserPropertyCategory",
                column: "PropertyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyCategory_UserId",
                table: "UserPropertyCategory",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantFeature");

            migrationBuilder.DropTable(
                name: "Beneficiary");

            migrationBuilder.DropTable(
                name: "ItemFeature");

            migrationBuilder.DropTable(
                name: "NotificationRecipient");

            migrationBuilder.DropTable(
                name: "NotificationSeener");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "PropertyFacility");

            migrationBuilder.DropTable(
                name: "PropertyFeature");

            migrationBuilder.DropTable(
                name: "UserItemCategory");

            migrationBuilder.DropTable(
                name: "UserPropertyCategory");

            migrationBuilder.DropTable(
                name: "Applicant");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Ownership");

            migrationBuilder.DropTable(
                name: "DealPayment");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "Feature");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "ItemRequest");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Deal");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "ItemCategory");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropTable(
                name: "PropertyCategory");
        }
    }
}
