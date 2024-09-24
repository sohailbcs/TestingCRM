using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class unicoursemigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourse_Courses_CourseId",
                table: "UniversityCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourse_Universities_UniversityId",
                table: "UniversityCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UniversityCourse",
                table: "UniversityCourse");

            migrationBuilder.RenameTable(
                name: "UniversityCourse",
                newName: "UniversityCourses");

            migrationBuilder.RenameIndex(
                name: "IX_UniversityCourse_CourseId",
                table: "UniversityCourses",
                newName: "IX_UniversityCourses_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniversityCourses",
                table: "UniversityCourses",
                columns: new[] { "UniversityId", "CourseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourses_Courses_CourseId",
                table: "UniversityCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourses_Universities_UniversityId",
                table: "UniversityCourses",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "UniversityId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourses_Courses_CourseId",
                table: "UniversityCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourses_Universities_UniversityId",
                table: "UniversityCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UniversityCourses",
                table: "UniversityCourses");

            migrationBuilder.RenameTable(
                name: "UniversityCourses",
                newName: "UniversityCourse");

            migrationBuilder.RenameIndex(
                name: "IX_UniversityCourses_CourseId",
                table: "UniversityCourse",
                newName: "IX_UniversityCourse_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UniversityCourse",
                table: "UniversityCourse",
                columns: new[] { "UniversityId", "CourseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourse_Courses_CourseId",
                table: "UniversityCourse",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourse_Universities_UniversityId",
                table: "UniversityCourse",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "UniversityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
