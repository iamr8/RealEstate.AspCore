﻿// <auto-generated />
using System;
using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealEstate.Domain;

namespace RealEstate.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RealEstate.Domain.Tables.Applicant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("ItemRequestId");

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("ItemRequestId");

                    b.HasIndex("UserId");

                    b.ToTable("Applicant");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.ApplicantFeature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicantId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FeatureId")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ApplicantId");

                    b.HasIndex("FeatureId");

                    b.ToTable("ApplicantFeature");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Beneficiary", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommissionPercent");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId")
                        .IsRequired();

                    b.Property<int>("TipPercent");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DealId");

                    b.HasIndex("UserId");

                    b.ToTable("Beneficiary");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Category", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Contact", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("MobileNumber")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.HasKey("Id");

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Deal", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("ItemRequestId");

                    b.HasKey("Id");

                    b.HasIndex("ItemRequestId")
                        .IsUnique()
                        .HasFilter("[ItemRequestId] IS NOT NULL");

                    b.ToTable("Deal");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.DealPayment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("CommissionPrice");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId")
                        .IsRequired();

                    b.Property<DateTime>("PayDate");

                    b.Property<string>("Text");

                    b.Property<decimal>("TipPrice");

                    b.HasKey("Id");

                    b.HasIndex("DealId");

                    b.ToTable("DealPayment");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.District", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("District");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Facility", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Facility");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Feature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Feature");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Item", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("PropertyId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.ItemFeature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FeatureId")
                        .IsRequired();

                    b.Property<string>("ItemId")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.HasIndex("ItemId");

                    b.ToTable("ItemFeature");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.ItemRequest", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<bool>("IsReject");

                    b.Property<string>("ItemId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("ItemRequest");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Log", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicantFeatureId");

                    b.Property<string>("ApplicantId");

                    b.Property<string>("BeneficiaryId");

                    b.Property<string>("CategoryId");

                    b.Property<string>("ContactId");

                    b.Property<string>("CreatorId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId");

                    b.Property<string>("DealPaymentId");

                    b.Property<string>("DistrictId");

                    b.Property<string>("FacilityId");

                    b.Property<string>("FeatureId");

                    b.Property<string>("ItemFeatureId");

                    b.Property<string>("ItemId");

                    b.Property<string>("ItemRequestId");

                    b.Property<string>("LogId");

                    b.Property<string>("OwnershipId");

                    b.Property<string>("PaymentId");

                    b.Property<string>("PictureId");

                    b.Property<string>("PropertyFacilityId");

                    b.Property<string>("PropertyFeatureId");

                    b.Property<string>("PropertyId");

                    b.Property<string>("PropertyOwnershipId");

                    b.Property<string>("SmsId");

                    b.Property<string>("SmsTemplateId");

                    b.Property<int>("Type");

                    b.Property<string>("UserId");

                    b.Property<string>("UserItemCategoryId");

                    b.Property<string>("UserPropertyCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("ApplicantFeatureId");

                    b.HasIndex("ApplicantId");

                    b.HasIndex("BeneficiaryId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ContactId");

                    b.HasIndex("DealId");

                    b.HasIndex("DealPaymentId");

                    b.HasIndex("DistrictId");

                    b.HasIndex("FacilityId");

                    b.HasIndex("FeatureId");

                    b.HasIndex("ItemFeatureId");

                    b.HasIndex("ItemId");

                    b.HasIndex("ItemRequestId");

                    b.HasIndex("LogId");

                    b.HasIndex("OwnershipId");

                    b.HasIndex("PaymentId");

                    b.HasIndex("PictureId");

                    b.HasIndex("PropertyFacilityId");

                    b.HasIndex("PropertyFeatureId");

                    b.HasIndex("PropertyId");

                    b.HasIndex("PropertyOwnershipId");

                    b.HasIndex("SmsId");

                    b.HasIndex("SmsTemplateId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserItemCategoryId");

                    b.HasIndex("UserPropertyCategoryId");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Ownership", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("Dong");

                    b.Property<string>("PropertyOwnershipId");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("PropertyOwnershipId");

                    b.ToTable("Ownership");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Payment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Text");

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Picture", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("DealId");

                    b.Property<string>("DealPaymentId");

                    b.Property<string>("File")
                        .IsRequired();

                    b.Property<string>("PaymentId");

                    b.Property<string>("PropertyId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("DealId");

                    b.HasIndex("DealPaymentId");

                    b.HasIndex("PaymentId");

                    b.HasIndex("PropertyId");

                    b.ToTable("Picture");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Property", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Alley");

                    b.Property<string>("BuildingName");

                    b.Property<string>("CategoryId");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Description");

                    b.Property<string>("DistrictId")
                        .IsRequired();

                    b.Property<int>("Flat");

                    b.Property<int>("Floor");

                    b.Property<IPoint>("Geolocation");

                    b.Property<string>("Number");

                    b.Property<string>("Street")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DistrictId");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.PropertyFacility", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FacilityId")
                        .IsRequired();

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FacilityId");

                    b.HasIndex("PropertyId");

                    b.ToTable("PropertyFacility");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.PropertyFeature", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FeatureId")
                        .IsRequired();

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.HasIndex("PropertyId");

                    b.ToTable("PropertyFeature");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.PropertyOwnership", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("PropertyId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PropertyId");

                    b.ToTable("PropertyOwnership");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Sms", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactId");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<int>("Provider");

                    b.Property<string>("Receiver");

                    b.Property<string>("ReferenceId");

                    b.Property<string>("Sender");

                    b.Property<string>("SmsTemplateId");

                    b.Property<string>("StatusJson");

                    b.Property<string>("Text");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("SmsTemplateId");

                    b.HasIndex("UserId");

                    b.ToTable("Sms");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.SmsTemplate", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("SmsTemplate");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<DateTime>("DateOfPay");

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<double>("FixedSalary");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Mobile")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.Property<int>("Role");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Mobile")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = "fbc6e313-98c0-4fe3-b234-6eabe2763f86",
                            Address = "باهنر",
                            DateOfPay = new DateTime(2019, 4, 2, 20, 33, 22, 766, DateTimeKind.Local).AddTicks(9302),
                            DateTime = new DateTime(2019, 4, 2, 20, 33, 22, 733, DateTimeKind.Local).AddTicks(9666),
                            FirstName = "هانی",
                            FixedSalary = 3600000.0,
                            LastName = "موسی زاده",
                            Mobile = "09166000341",
                            Password = "YmAdyc6Ph9PNcJOLeira6w==",
                            Phone = "33379367",
                            Role = 2,
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.UserItemCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("UserItemCategory");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.UserPropertyCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<DateTime>("DateTime")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPropertyCategory");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Applicant", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Contact", "Contact")
                        .WithMany("Applicants")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.ItemRequest", "ItemRequest")
                        .WithMany("Applicants")
                        .HasForeignKey("ItemRequestId");

                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("Applicants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.ApplicantFeature", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Applicant", "Applicant")
                        .WithMany("ApplicantFeatures")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.Feature", "Feature")
                        .WithMany("ApplicantFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Beneficiary", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Deal", "Deal")
                        .WithMany("Beneficiaries")
                        .HasForeignKey("DealId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("Beneficiaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Deal", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.ItemRequest", "ItemRequest")
                        .WithOne("Deal")
                        .HasForeignKey("RealEstate.Domain.Tables.Deal", "ItemRequestId");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.DealPayment", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Deal", "Deal")
                        .WithMany("DealPayments")
                        .HasForeignKey("DealId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Item", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.Property", "Property")
                        .WithMany("Items")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.ItemFeature", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Feature", "Feature")
                        .WithMany("ItemFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.Item", "Item")
                        .WithMany("ItemFeatures")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.ItemRequest", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Item", "Item")
                        .WithMany("ItemRequests")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Log", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.ApplicantFeature", "ApplicantFeature")
                        .WithMany("Logs")
                        .HasForeignKey("ApplicantFeatureId");

                    b.HasOne("RealEstate.Domain.Tables.Applicant", "Applicant")
                        .WithMany("Logs")
                        .HasForeignKey("ApplicantId");

                    b.HasOne("RealEstate.Domain.Tables.Beneficiary", "Beneficiary")
                        .WithMany("Logs")
                        .HasForeignKey("BeneficiaryId");

                    b.HasOne("RealEstate.Domain.Tables.Category", "Category")
                        .WithMany("Logs")
                        .HasForeignKey("CategoryId");

                    b.HasOne("RealEstate.Domain.Tables.Contact", "Contact")
                        .WithMany("Logs")
                        .HasForeignKey("ContactId");

                    b.HasOne("RealEstate.Domain.Tables.Deal", "Deal")
                        .WithMany("Logs")
                        .HasForeignKey("DealId");

                    b.HasOne("RealEstate.Domain.Tables.DealPayment", "DealPayment")
                        .WithMany("Logs")
                        .HasForeignKey("DealPaymentId");

                    b.HasOne("RealEstate.Domain.Tables.District", "District")
                        .WithMany("Logs")
                        .HasForeignKey("DistrictId");

                    b.HasOne("RealEstate.Domain.Tables.Facility", "Facility")
                        .WithMany("Logs")
                        .HasForeignKey("FacilityId");

                    b.HasOne("RealEstate.Domain.Tables.Feature", "Feature")
                        .WithMany("Logs")
                        .HasForeignKey("FeatureId");

                    b.HasOne("RealEstate.Domain.Tables.ItemFeature", "ItemFeature")
                        .WithMany("Logs")
                        .HasForeignKey("ItemFeatureId");

                    b.HasOne("RealEstate.Domain.Tables.Item", "Item")
                        .WithMany("Logs")
                        .HasForeignKey("ItemId");

                    b.HasOne("RealEstate.Domain.Tables.ItemRequest", "ItemRequest")
                        .WithMany("Logs")
                        .HasForeignKey("ItemRequestId");

                    b.HasOne("RealEstate.Domain.Tables.Log")
                        .WithMany("Logs")
                        .HasForeignKey("LogId");

                    b.HasOne("RealEstate.Domain.Tables.Ownership", "Ownership")
                        .WithMany("Logs")
                        .HasForeignKey("OwnershipId");

                    b.HasOne("RealEstate.Domain.Tables.Payment", "Payment")
                        .WithMany("Logs")
                        .HasForeignKey("PaymentId");

                    b.HasOne("RealEstate.Domain.Tables.Picture", "Picture")
                        .WithMany("Logs")
                        .HasForeignKey("PictureId");

                    b.HasOne("RealEstate.Domain.Tables.PropertyFacility", "PropertyFacility")
                        .WithMany("Logs")
                        .HasForeignKey("PropertyFacilityId");

                    b.HasOne("RealEstate.Domain.Tables.PropertyFeature", "PropertyFeature")
                        .WithMany("Logs")
                        .HasForeignKey("PropertyFeatureId");

                    b.HasOne("RealEstate.Domain.Tables.Property", "Property")
                        .WithMany("Logs")
                        .HasForeignKey("PropertyId");

                    b.HasOne("RealEstate.Domain.Tables.PropertyOwnership", "PropertyOwnership")
                        .WithMany("Logs")
                        .HasForeignKey("PropertyOwnershipId");

                    b.HasOne("RealEstate.Domain.Tables.Sms")
                        .WithMany("Logs")
                        .HasForeignKey("SmsId");

                    b.HasOne("RealEstate.Domain.Tables.SmsTemplate", "SmsTemplate")
                        .WithMany("Logs")
                        .HasForeignKey("SmsTemplateId");

                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("Logs")
                        .HasForeignKey("UserId");

                    b.HasOne("RealEstate.Domain.Tables.UserItemCategory", "UserItemCategory")
                        .WithMany("Logs")
                        .HasForeignKey("UserItemCategoryId");

                    b.HasOne("RealEstate.Domain.Tables.UserPropertyCategory", "UserPropertyCategory")
                        .WithMany("Logs")
                        .HasForeignKey("UserPropertyCategoryId");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Ownership", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Contact", "Contact")
                        .WithMany("Ownerships")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.PropertyOwnership", "PropertyOwnership")
                        .WithMany("Ownerships")
                        .HasForeignKey("PropertyOwnershipId");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Payment", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Picture", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Deal", "Deal")
                        .WithMany("Pictures")
                        .HasForeignKey("DealId");

                    b.HasOne("RealEstate.Domain.Tables.DealPayment", "DealPayment")
                        .WithMany("Pictures")
                        .HasForeignKey("DealPaymentId");

                    b.HasOne("RealEstate.Domain.Tables.Payment", "Payment")
                        .WithMany("Pictures")
                        .HasForeignKey("PaymentId");

                    b.HasOne("RealEstate.Domain.Tables.Property", "Property")
                        .WithMany("Pictures")
                        .HasForeignKey("PropertyId");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Property", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Category", "Category")
                        .WithMany("Properties")
                        .HasForeignKey("CategoryId");

                    b.HasOne("RealEstate.Domain.Tables.District", "District")
                        .WithMany("Properties")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.PropertyFacility", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Facility", "Facility")
                        .WithMany("PropertyFacilities")
                        .HasForeignKey("FacilityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.Property", "Property")
                        .WithMany("PropertyFacilities")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.PropertyFeature", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Feature", "Feature")
                        .WithMany("PropertyFeatures")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.Property", "Property")
                        .WithMany("PropertyFeatures")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.PropertyOwnership", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Property", "Property")
                        .WithMany("PropertyOwnerships")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.Sms", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Contact", "Contact")
                        .WithMany("Smses")
                        .HasForeignKey("ContactId");

                    b.HasOne("RealEstate.Domain.Tables.SmsTemplate", "SmsTemplate")
                        .WithMany("Smses")
                        .HasForeignKey("SmsTemplateId");

                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("Smses")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.UserItemCategory", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Category", "Category")
                        .WithMany("UserItemCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("UserItemCategories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstate.Domain.Tables.UserPropertyCategory", b =>
                {
                    b.HasOne("RealEstate.Domain.Tables.Category", "Category")
                        .WithMany("UserPropertyCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RealEstate.Domain.Tables.User", "User")
                        .WithMany("UserPropertyCategories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
