using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Itsm.Migrations
{
    /// <inheritdoc />
    public partial class SlaBilingual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalateTo",
                table: "ProjectSlas");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "ProjectSlas",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "ProjectSlas");

            migrationBuilder.AddColumn<string>(
                name: "EscalateTo",
                table: "ProjectSlas",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
