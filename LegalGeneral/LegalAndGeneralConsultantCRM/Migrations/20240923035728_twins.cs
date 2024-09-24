using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class twins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Applicantstatus",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudyGap",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Applicantstatus",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Leads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cnic",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudyGap",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LeadHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attachment5",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attachment6",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title5",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title6",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Applicationprocessor",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Applicantstatus",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudyGap",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Applicantstatus",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Cnic",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "StudyGap",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LeadHistories");

            migrationBuilder.DropColumn(
                name: "attachment5",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "attachment6",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "title5",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "title6",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "Applicationprocessor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");
        }
    }
}
