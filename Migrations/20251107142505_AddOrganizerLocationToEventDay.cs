using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgilityScore.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerLocationToEventDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "EventDays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Organizer",
                table: "EventDays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartOrder",
                table: "EventDays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "EventDays");

            migrationBuilder.DropColumn(
                name: "Organizer",
                table: "EventDays");

            migrationBuilder.DropColumn(
                name: "StartOrder",
                table: "EventDays");
        }
    }
}
