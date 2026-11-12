using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itsm.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SlaBreachSent",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SlaWarningSent",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlaBreachSent",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaWarningSent",
                table: "Tickets");
        }
    }
}
