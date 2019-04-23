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
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    Value = table.Column<double>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Employee_EmployeeId",
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
                    StatusJson = table.Column<string>(nullable: true),
                    CustomerId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sms_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sms_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Description = table.Column<string>(nullable: false),
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
                    DealId = table.Column<string>(nullable: true)
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
                    { "fcdf54c9-2e82-4384-acf3-4be023803576", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "خرید و فروش", 0 },
                    { "18dada49-eb92-4435-9d68-4513226376fa", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "زمین", 1 },
                    { "296b08fe-3d69-45d5-b8c7-1b2aa5205a0c", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "ویلایی", 1 },
                    { "07d0fc97-1cf2-4688-aee8-24919f68b6bd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "مشارکت در ساخت", 0 },
                    { "7ee3c867-cb10-4668-8f9c-e0b6768aca45", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "رهن و اجاره", 0 },
                    { "25b18d5b-f861-4b2c-bf82-a4875db8eb26", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "رهن کامل", 0 },
                    { "9c356476-d411-4e7f-a812-39b59f514bad", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "آپارتمان", 1 },
                    { "dd0a18d7-7770-4d8d-bb95-3caf4bed298e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "مغازه", 1 }
                });

            migrationBuilder.InsertData(
                table: "District",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "dbaa9cd6-bff6-43f2-b0a7-0f8626894964", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "زیتون کارمندی" },
                    { "f65efe6f-d7e5-468b-b824-6d5b90c146f2", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "زیتون کارگری" },
                    { "245fc791-8e69-400c-a9fe-b62f7edcd639", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "باهنر" },
                    { "9370f570-144f-4726-848c-55998ff05592", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کیان آباد" },
                    { "2c5e2312-a2f9-4828-8335-e4b12f12bd0b", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کیانپارس" },
                    { "36010ce4-8c6e-48c9-a0f1-9a72c321add2", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "ملیراه" }
                });

            migrationBuilder.InsertData(
                table: "Division",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "d196f024-32c7-4c7a-8e7b-f0ba53356583", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کارواش" },
                    { "0eb73618-1fd1-4bd0-9bdf-d1b8203905c8", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "املاک" }
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "Address", "Audit", "FirstName", "LastName", "Mobile", "Phone" },
                values: new object[] { "e17a64aa-9732-4e11-ba0a-26ee515d8dd5", "باهنر", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "هانی", "موسی زاده", "09166000341", "33379367" });

            migrationBuilder.InsertData(
                table: "Facility",
                columns: new[] { "Id", "Audit", "Name" },
                values: new object[,]
                {
                    { "bda52f5f-76b6-49f1-a0b8-a5626d7bf62e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "سالن بدنسازی" },
                    { "2dd39a45-fda3-4b60-b58f-bda831d3a7fd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "پارکینگ" },
                    { "ea761002-3277-491e-bbc5-08d1b7d1d2a7", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "آسانسور" },
                    { "538e01d9-fbfe-4bc9-8e33-a789edf02cbc", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "سالن همایش" },
                    { "9098ca45-b036-4c98-a41e-f3ff8dc199cd", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "آنتن مرکزی" }
                });

            migrationBuilder.InsertData(
                table: "Feature",
                columns: new[] { "Id", "Audit", "Name", "Type" },
                values: new object[,]
                {
                    { "9c34c06c-0989-42b4-930e-db50260bb2f9", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "بر زمین", 1 },
                    { "9eb642bd-5320-4b27-998e-8f90fb95116e", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "قیمت نهایی", 0 },
                    { "8d4576a7-7728-47de-8c45-e348e30e67fb", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "بودجه", 2 },
                    { "f74540f2-140b-4b34-9789-b4823bb940bb", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "متراژ", 1 },
                    { "dde31d12-397e-4a0c-8fd7-d87efd345db6", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "پیش پرداخت", 2 },
                    { "28f4e37a-aeae-4e00-92e8-f2a0a28f8b40", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "قیمت هر متر", 0 },
                    { "aba3615a-c4ab-443d-af8a-a45ccef6d613", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "تعداد خواب", 1 },
                    { "32126744-4fed-406d-8a44-e772611270ad", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "کرایه", 2 }
                });

            migrationBuilder.InsertData(
                table: "EmployeeDivision",
                columns: new[] { "Id", "Audit", "DivisionId", "EmployeeId" },
                values: new object[] { "6a7e9e40-3535-465a-883f-86fa21cb58d7", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "0eb73618-1fd1-4bd0-9bdf-d1b8203905c8", "e17a64aa-9732-4e11-ba0a-26ee515d8dd5" });

            migrationBuilder.InsertData(
                table: "EmployeeStatus",
                columns: new[] { "Id", "Audit", "EmployeeId", "Status" },
                values: new object[] { "77cf3bf9-013f-46e9-8889-7b615e8a8c82", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "e17a64aa-9732-4e11-ba0a-26ee515d8dd5", 0 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Audit", "EmployeeId", "Password", "Role", "Username" },
                values: new object[] { "85bf213c-2af9-4e36-ae45-4d4f5dfd2144", "[{\"i\":null,\"n\":\"آرش شبه\",\"m\":\"09364091209\",\"d\":\"2019-04-24T00:34:29.7761901+04:30\",\"t\":0}]", "e17a64aa-9732-4e11-ba0a-26ee515d8dd5", "YmAdyc6Ph9PNcJOLeira6w==", 2, "admin" });

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
                name: "IX_Payment_EmployeeId",
                table: "Payment",
                column: "EmployeeId");

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
                name: "IX_Sms_CustomerId",
                table: "Sms",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sms_EmployeeId",
                table: "Sms",
                column: "EmployeeId");

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
                name: "Sms");

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
