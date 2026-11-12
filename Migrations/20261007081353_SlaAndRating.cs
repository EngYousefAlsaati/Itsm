using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itsm.Migrations
{
    /// <inheritdoc />
    public partial class SlaAndRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DecidedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecidedBy",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RatedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RatingComment",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaStoppedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Solution",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianNotes",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DecidedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DecidedBy",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RatedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RatingComment",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaStoppedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Solution",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TechnicianNotes",
                table: "Tickets");
        }
    }
}
