using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game_of_Drones.Migrations
{
    public partial class SetKillMoveIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 1,
                column: "KillMoveId",
                value: 3); // Piedra mata a Tijera

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 2,
                column: "KillMoveId",
                value: 1); // Papel mata a Piedra

            migrationBuilder.UpdateData(
                table: "Moves",
                keyColumn: "Id",
                keyValue: 3,
                column: "KillMoveId",
                value: 2); // Tijera mata a Papel
        }
    }
}