using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class incentivemigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incentive_Courses_CourseId",
                table: "Incentive");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Incentive",
                table: "Incentive");

            migrationBuilder.RenameTable(
                name: "Incentive",
                newName: "Incentives");

            migrationBuilder.RenameIndex(
                name: "IX_Incentive_CourseId",
                table: "Incentives",
                newName: "IX_Incentives_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Incentives",
                table: "Incentives",
                column: "IncentiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incentives_Courses_CourseId",
                table: "Incentives",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incentives_Courses_CourseId",
                table: "Incentives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Incentives",
                table: "Incentives");

            migrationBuilder.RenameTable(
                name: "Incentives",
                newName: "Incentive");

            migrationBuilder.RenameIndex(
                name: "IX_Incentives_CourseId",
                table: "Incentive",
                newName: "IX_Incentive_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Incentive",
                table: "Incentive",
                column: "IncentiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incentive_Courses_CourseId",
                table: "Incentive",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
