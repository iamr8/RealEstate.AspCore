using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealEstate.Web.Migrations
{
    public partial class SomeSwaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Ownership");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Ownership");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Ownership");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Applicant");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Applicant");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contact",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Contact",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Ownership",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Ownership",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Ownership",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Applicant",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Applicant",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Applicant",
                nullable: true);
        }
    }
}
