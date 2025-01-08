using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class CorrectForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "NGOs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NGOs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_NGOs_AccountId",
                table: "NGOs",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_NGOs_Accounts_AccountId",
                table: "NGOs",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NGOs_Accounts_AccountId",
                table: "NGOs");

            migrationBuilder.DropIndex(
                name: "IX_NGOs_AccountId",
                table: "NGOs");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "NGOs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NGOs");
        }
    }
}
