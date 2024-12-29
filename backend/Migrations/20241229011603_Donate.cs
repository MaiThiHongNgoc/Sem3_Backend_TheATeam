using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Donate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetAmount",
                table: "ProgramDonations");

            migrationBuilder.AddColumn<decimal>(
                name: "TargetAmount",
                table: "Program1s",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetAmount",
                table: "Program1s");

            migrationBuilder.AddColumn<decimal>(
                name: "TargetAmount",
                table: "ProgramDonations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
