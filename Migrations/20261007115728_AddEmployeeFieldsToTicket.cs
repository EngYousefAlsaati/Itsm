using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itsm.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeFieldsToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeEmail",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Organization",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeEmail",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Organization",
                table: "Tickets");
        }
    }
}
