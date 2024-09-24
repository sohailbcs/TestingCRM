using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalAndGeneralConsultantCRM.Migrations
{
    public partial class attachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             

            migrationBuilder.AddColumn<string>(
                name: "attachment2",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attachment3",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attachment4",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title1",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title2",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title3",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title4",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: true);

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attachment2",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "attachment3",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "attachment4",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "title1",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "title2",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "title3",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "title4",
                table: "Educations");

            

             
        }
    }
}
