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
                    Barcode = table.Column<string>(nullable: true)
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
                    Phone = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false)
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
                name: "DealPayment",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
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
                    Price = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
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
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false),
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
                    Text = table.Column<string>(nullable: true),
                    AlarmTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminder", x => x.Id);
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
                name: "Check",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Audit = table.Column<string>(nullable: true),
                    PayDate = table.Column<DateTime>(nullable: false),
                    Bank = table.Column<string>(nullable: true),
                    CheckNumber = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DealId = table.Column<string>(nullable: true),
                    ReminderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Check", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Check_Deal_DealId",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Check_Reminder_ReminderId",
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
                    DealPaymentId = table.Column<string>(nullable: true),
                    CheckId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Picture_Check_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Check",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                });

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
                name: "IX_Check_DealId",
                table: "Check",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Check_ReminderId",
                table: "Check",
                column: "ReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_DealPayment_DealId",
                table: "DealPayment",
                column: "DealId");

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
                name: "IX_Picture_CheckId",
                table: "Picture",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_DealId",
                table: "Picture",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Picture_DealPaymentId",
                table: "Picture",
                column: "DealPaymentId");

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
                name: "Check");

            migrationBuilder.DropTable(
                name: "DealPayment");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "Feature");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Reminder");

            migrationBuilder.DropTable(
                name: "Deal");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
