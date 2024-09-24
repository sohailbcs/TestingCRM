using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class dbupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "UniversityCourses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversityCourses_DomainId",
                table: "UniversityCourses",
                column: "DomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourses_Domain_DomainId",
                table: "UniversityCourses",
                column: "DomainId",
                principalTable: "Domain",
                principalColumn: "DomainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourses_Domain_DomainId",
                table: "UniversityCourses");

            migrationBuilder.DropIndex(
                name: "IX_UniversityCourses_DomainId",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "UniversityCourses");
        }
    }
}
