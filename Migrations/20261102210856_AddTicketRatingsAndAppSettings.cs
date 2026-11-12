using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itsm.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketRatingsAndAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "TicketRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    TechnicianEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EvaluatorRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EvaluatorEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EvaluatorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EvaluatorContact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ScoresCsv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Average = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdminReply = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AdminReplyAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRatings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketRatings_TicketId_EvaluatorRole",
                table: "TicketRatings",
                columns: new[] { "TicketId", "EvaluatorRole" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "TicketRatings");
        }
    }
}
