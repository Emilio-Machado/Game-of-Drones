using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game_of_Drones.Migrations
{
    public partial class UpgradeToNet10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Players",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Players",
                newName: "ID");
        }
    }
}