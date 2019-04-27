using System;
using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deal",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    CommissionPrice = table.Column<decimal>(nullable: false),
                    TipPrice = table.Column<decimal>(nullable: false)
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
                    Audit = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_District", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Mobile = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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
                    Audit = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feature", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Receiver = table.Column<string>(nullable: true),
                    ReferenceId = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Provider = table.Column<int>(nullable: false),
                    StatusJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: false),
                    Alley = table.Column<string>(nullable: true),
                    BuildingName = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    Floor = table.Column<int>(nullable: false),
                    Flat = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DistrictId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: true),
                    Geolocation = table.Column<IPoint>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Property_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Property_District_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDivision",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: false),
                    DivisionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDivision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDivision_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeDivision_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeStatus",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeStatus_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FixedSalary",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedSalary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedSalary_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Insurance",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insurance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Insurance_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leave",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leave_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagementPercent",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Percent = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementPercent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementPercent_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presence",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    Minute = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presence_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    SmsId = table.Column<string>(nullable: true),
                    CheckoutId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Payment_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Sms_SmsId",
                        column: x => x.SmsId,
                        principalTable: "Sms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PropertyId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
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
                name: "PropertyFacility",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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
                    Audit = table.Column<string>(nullable: true),
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
                name: "PropertyOwnership",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "Beneficiary",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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
                name: "Reminder",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    CheckBank = table.Column<string>(nullable: true),
                    CheckNumber = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    DealId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminder_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reminder_User_UserId",
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
                    Audit = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserItemCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserItemCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
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
                    Audit = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertyCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPropertyCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
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
                name: "Applicant",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    CustomerId = table.Column<string>(nullable: false),
                    ItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicant_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applicant_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
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
                name: "DealRequest",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ItemId = table.Column<string>(nullable: false),
                    DealId = table.Column<string>(nullable: true),
                    SmsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealRequest_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DealRequest_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DealRequest_Sms_SmsId",
                        column: x => x.SmsId,
                        principalTable: "Sms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemFeature",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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
                name: "Ownership",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Dong = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    PropertyOwnershipId = table.Column<string>(nullable: true),
                    CustomerId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ownership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ownership_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ownership_PropertyOwnership_PropertyOwnershipId",
                        column: x => x.PropertyOwnershipId,
                        principalTable: "PropertyOwnership",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    File = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    PropertyId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true),
                    DealId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    ReminderId = table.Column<string>(nullable: true)
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
                        name: "FK_Picture_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
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
                    table.ForeignKey(
                        name: "FK_Picture_Reminder_ReminderId",
                        column: x => x.ReminderId,
                        principalTable: "Reminder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantFeature",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "1943a814-7c25-4569-b528-8e96c4b5d3f8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "خرید و فروش", 0 },
                    { "8f19a3ee-b39a-47f1-84d3-08ca6f62278b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "زمین", 1 },
                    { "91144dc3-70e3-44a8-904c-cc8bf12aaa4c", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "ویلایی", 1 },
                    { "d03ef728-2932-480f-bfa1-11c77b2e8767", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "مشارکت در ساخت", 0 },
                    { "199acde7-7244-42c8-a6e7-eab43d0a9eee", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "رهن و اجاره", 0 },
                    { "5816a67b-eb6a-464e-b6c9-7f0520eef95e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "رهن کامل", 0 },
                    { "826e9eaf-40f7-4adc-9a1d-b84ad151e176", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "آپارتمان", 1 },
                    { "29dfac27-765c-4839-b77b-0afc7b6450e3", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "مغازه", 1 }
                });

            migrationBuilder.InsertData(
                table: "District",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "b79d7dc7-0f26-448a-baf7-87b394a3c41c", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "زیتون کارمندی" },
                    { "fb5298c9-b9cf-409a-b631-83e50d174659", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "زیتون کارگری" },
                    { "fd487831-2da5-4e3f-8411-b68a3ef6310a", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "باهنر" },
                    { "e23e453d-df4f-47fc-b569-e18e29f849f8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "کیان آباد" },
                    { "3c53d525-487e-4163-b391-23d890888999", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "کیانپارس" },
                    { "1583db20-61fa-49fa-8dc8-828a166dd711", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "ملیراه" }
                });

            migrationBuilder.InsertData(
                table: "Division",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "955fcc87-c851-4f7b-bb01-58ab853da6fc", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "کارواش" },
                    { "c2a55dcd-f3db-4fc3-853c-496ad8b43a1d", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "املاک" }
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "Address", "Audit", "FirstName", "LastName", "Mobile", "Phone" },
                values: new object[] { "f60fe209-c03b-48e5-9bb2-926184ed0c11", "باهنر", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "هانی", "موسی زاده", "09166000341", "33379367" });

            migrationBuilder.InsertData(
                table: "Facility",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "f36231da-d068-4c4d-a2f1-62010a1e06c5", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "سالن بدنسازی" },
                    { "ac5e7676-dd9b-4674-a636-a4c50e8e5db7", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "پارکینگ" },
                    { "e019156a-79cd-4695-8900-ba1b0113267b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "آسانسور" },
                    { "328b6393-ec90-440b-94d6-bc75957f3a39", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "سالن همایش" },
                    { "b789b07b-9a34-4a51-a4bb-51acf4f036a6", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "آنتن مرکزی" }
                });

            migrationBuilder.InsertData(
                table: "Feature",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "93179c86-a0db-40cf-a0e2-d26c70a8de45", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "بر زمین", 1 },
                    { "54a0b920-c17f-4ff2-9c51-f9551159026a", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "قیمت نهایی", 0 },
                    { "ac1ba7ca-7054-4f46-a234-c3d6fd99b442", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "بودجه", 2 },
                    { "15bf9d15-07bc-4f3c-8339-8192c8fd0c18", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "متراژ", 1 },
                    { "22f68cda-29f2-4cc0-bb0f-e578defb85d1", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "پیش پرداخت", 2 },
                    { "01cb6a1d-959d-4abb-8488-f10ab09bd8a8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "قیمت هر متر", 0 },
                    { "b35f4bef-925e-415b-b8f1-19f6df02e6ac", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "تعداد خواب", 1 },
                    { "02cbebcc-610a-4bd2-8e27-e2d50b13587f", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "کرایه", 2 }
                });

            migrationBuilder.InsertData(
                table: "EmployeeDivision",
                columns: new[] { "Id", "Audit", "DivisionId", "EmployeeId" },
                values: new object[] { "4c86edfc-64e5-43c9-981e-83e71c64ca57", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "c2a55dcd-f3db-4fc3-853c-496ad8b43a1d", "f60fe209-c03b-48e5-9bb2-926184ed0c11" });

            migrationBuilder.InsertData(
                table: "EmployeeStatus",
                columns: new[] { "Id", "Audit", "EmployeeId", "Status" },
                values: new object[] { "7d8f0e43-d368-46a1-85c7-789fa2737a10", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "f60fe209-c03b-48e5-9bb2-926184ed0c11", 0 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Audit", "EmployeeId", "Password", "Role", "Username" },
                values: new object[] { "f19fb481-805f-4ef7-b63d-3be132a83215", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-25T16:48:03.9433293+04:30\",\"t\":0}]", "f60fe209-c03b-48e5-9bb2-926184ed0c11", "YmAdyc6Ph9PNcJOLeira6w==", 2, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_CustomerId",
                table: "Applicant",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicant_ItemId",
                table: "Applicant",
                column: "ItemId");

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
                name: "IX_DealRequest_DealId",
                table: "DealRequest",
                column: "DealId",
                unique: true,
                filter: "[DealId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DealRequest_ItemId",
                table: "DealRequest",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DealRequest_SmsId",
                table: "DealRequest",
                column: "SmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Mobile",
                table: "Employee",
                column: "Mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDivision_DivisionId",
                table: "EmployeeDivision",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDivision_EmployeeId",
                table: "EmployeeDivision",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStatus_EmployeeId",
                table: "EmployeeStatus",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedSalary_EmployeeId",
                table: "FixedSalary",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurance_EmployeeId",
                table: "Insurance",
                column: "EmployeeId");

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
                name: "IX_Leave_EmployeeId",
                table: "Leave",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementPercent_EmployeeId",
                table: "ManagementPercent",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ownership_CustomerId",
                table: "Ownership",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ownership_PropertyOwnershipId",
                table: "Ownership",
                column: "PropertyOwnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CheckoutId",
                table: "Payment",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_EmployeeId",
                table: "Payment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_SmsId",
                table: "Payment",
                column: "SmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_DealId",
                table: "Picture",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_EmployeeId",
                table: "Picture",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_PaymentId",
                table: "Picture",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_PropertyId",
                table: "Picture",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_ReminderId",
                table: "Picture",
                column: "ReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_Presence_EmployeeId",
                table: "Presence",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_CategoryId",
                table: "Property",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_DistrictId",
                table: "Property",
                column: "DistrictId");

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
                name: "IX_PropertyOwnership_PropertyId",
                table: "PropertyOwnership",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminder_DealId",
                table: "Reminder",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminder_UserId",
                table: "Reminder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_EmployeeId",
                table: "User",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserItemCategory_CategoryId",
                table: "UserItemCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserItemCategory_UserId",
                table: "UserItemCategory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPropertyCategory_CategoryId",
                table: "UserPropertyCategory",
                column: "CategoryId");

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
                name: "DealRequest");

            migrationBuilder.DropTable(
                name: "EmployeeDivision");

            migrationBuilder.DropTable(
                name: "EmployeeStatus");

            migrationBuilder.DropTable(
                name: "FixedSalary");

            migrationBuilder.DropTable(
                name: "Insurance");

            migrationBuilder.DropTable(
                name: "ItemFeature");

            migrationBuilder.DropTable(
                name: "Leave");

            migrationBuilder.DropTable(
                name: "ManagementPercent");

            migrationBuilder.DropTable(
                name: "Ownership");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "Presence");

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
                name: "Division");

            migrationBuilder.DropTable(
                name: "PropertyOwnership");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Reminder");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "Feature");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Sms");

            migrationBuilder.DropTable(
                name: "Deal");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "District");
        }
    }
}
