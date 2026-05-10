using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConferenceManagement.Dal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSessionTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "duration",
                table: "sessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_time",
                table: "sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "start_time",
                table: "sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_time",
                table: "sessions");

            migrationBuilder.DropColumn(
                name: "start_time",
                table: "sessions");

            migrationBuilder.AddColumn<int>(
                name: "duration",
                table: "sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
