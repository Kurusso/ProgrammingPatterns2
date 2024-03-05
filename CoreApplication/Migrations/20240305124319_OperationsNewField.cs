using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreApplication.Migrations
{
    /// <inheritdoc />
    public partial class OperationsNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MoneyAmmountInAccountCurrency",
                table: "Operations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyAmmountInAccountCurrency",
                table: "Operations");
        }
    }
}
