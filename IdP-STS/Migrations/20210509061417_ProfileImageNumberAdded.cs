using Microsoft.EntityFrameworkCore.Migrations;

namespace STS.Migrations
{
    public partial class ProfileImageNumberAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileImageNumber",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageNumber",
                table: "AspNetUsers");
        }
    }
}
