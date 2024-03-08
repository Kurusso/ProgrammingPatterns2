using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditApplication.Migrations
{
    /// <inheritdoc />
    public partial class creditRateModelChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyAmount",
                table: "CreditRates");

            migrationBuilder.DropColumn(
                name: "MonthPayAmount",
                table: "CreditRates");

            migrationBuilder.AddColumn<int>(
                name: "FullMoneyAmount",
                table: "Credits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthPayAmount",
                table: "Credits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CreditRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullMoneyAmount",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "MonthPayAmount",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CreditRates");

            migrationBuilder.AddColumn<int>(
                name: "MoneyAmount",
                table: "CreditRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthPayAmount",
                table: "CreditRates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
