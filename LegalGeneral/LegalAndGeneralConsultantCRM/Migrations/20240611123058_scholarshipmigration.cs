using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class scholarshipmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scholarship_Courses_CourseId",
                table: "Scholarship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scholarship",
                table: "Scholarship");

            migrationBuilder.RenameTable(
                name: "Scholarship",
                newName: "Scholarships");

            migrationBuilder.RenameIndex(
                name: "IX_Scholarship_CourseId",
                table: "Scholarships",
                newName: "IX_Scholarships_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scholarships",
                table: "Scholarships",
                column: "ScholarshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scholarships_Courses_CourseId",
                table: "Scholarships",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scholarships_Courses_CourseId",
                table: "Scholarships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scholarships",
                table: "Scholarships");

            migrationBuilder.RenameTable(
                name: "Scholarships",
                newName: "Scholarship");

            migrationBuilder.RenameIndex(
                name: "IX_Scholarships_CourseId",
                table: "Scholarship",
                newName: "IX_Scholarship_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scholarship",
                table: "Scholarship",
                column: "ScholarshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scholarship_Courses_CourseId",
                table: "Scholarship",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
