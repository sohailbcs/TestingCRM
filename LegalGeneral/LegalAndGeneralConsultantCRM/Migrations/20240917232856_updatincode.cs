using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class updatincode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "UniversityCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intake1",
                table: "UniversityCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intake2",
                table: "UniversityCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intake3",
                table: "UniversityCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "interestedcountry",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "interestedprogram",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "yearCompletion",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DurationInYears",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "Intake1",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "Intake2",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "Intake3",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "interestedcountry",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "interestedprogram",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "yearCompletion",
                table: "Leads");

            migrationBuilder.AlterColumn<int>(
                name: "DurationInYears",
                table: "Courses",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
