using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class scholarshipuniidmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniversityId",
                table: "Scholarships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scholarships_UniversityId",
                table: "Scholarships",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scholarships_Universities_UniversityId",
                table: "Scholarships",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "UniversityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scholarships_Universities_UniversityId",
                table: "Scholarships");

            migrationBuilder.DropIndex(
                name: "IX_Scholarships_UniversityId",
                table: "Scholarships");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Scholarships");
        }
    }
}
