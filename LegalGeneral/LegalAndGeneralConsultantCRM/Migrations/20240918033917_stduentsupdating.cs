using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class stduentsupdating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "UniversityCourses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domains",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intake",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Domain",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversityCourses_StudentId",
                table: "UniversityCourses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Domain_StudentId",
                table: "Domain",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Domain_Students_StudentId",
                table: "Domain",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourses_Students_StudentId",
                table: "UniversityCourses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Domain_Students_StudentId",
                table: "Domain");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourses_Students_StudentId",
                table: "UniversityCourses");

            migrationBuilder.DropIndex(
                name: "IX_UniversityCourses_StudentId",
                table: "UniversityCourses");

            migrationBuilder.DropIndex(
                name: "IX_Domain_StudentId",
                table: "Domain");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "Domains",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Intake",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Domain");
        }
    }
}
