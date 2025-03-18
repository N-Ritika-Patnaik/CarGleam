using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarGleam.Migrations
{
    /// <inheritdoc />
    public partial class sixteenth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Bookings");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Machines",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "MachineType",
                table: "Machines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Machines");

            migrationBuilder.DropColumn(
                name: "MachineType",
                table: "Machines");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Bookings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
