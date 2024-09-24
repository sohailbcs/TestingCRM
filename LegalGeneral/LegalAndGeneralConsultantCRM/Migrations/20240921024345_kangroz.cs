using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class kangroz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "University",
                table: "Leads");

            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "UniversityCourses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniversityId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniversityCourses_LeadId",
                table: "UniversityCourses",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_DomainId",
                table: "Leads",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_UniversityId",
                table: "Leads",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Domain_DomainId",
                table: "Leads",
                column: "DomainId",
                principalTable: "Domain",
                principalColumn: "DomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Universities_UniversityId",
                table: "Leads",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UniversityCourses_Leads_LeadId",
                table: "UniversityCourses",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "LeadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Domain_DomainId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Universities_UniversityId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_UniversityCourses_Leads_LeadId",
                table: "UniversityCourses");

            migrationBuilder.DropIndex(
                name: "IX_UniversityCourses_LeadId",
                table: "UniversityCourses");

            migrationBuilder.DropIndex(
                name: "IX_Leads_DomainId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_UniversityId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "UniversityCourses");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Leads");

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
