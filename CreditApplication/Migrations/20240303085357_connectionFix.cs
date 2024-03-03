using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditApplication.Migrations
{
    /// <inheritdoc />
    public partial class connectionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_CreditRateDTO_CreditRateId",
                table: "Credits");

            migrationBuilder.DropTable(
                name: "CreditRateDTO");

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_CreditRates_CreditRateId",
                table: "Credits",
                column: "CreditRateId",
                principalTable: "CreditRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_CreditRates_CreditRateId",
                table: "Credits");

            migrationBuilder.CreateTable(
                name: "CreditRateDTO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoneyAmount = table.Column<int>(type: "int", nullable: false),
                    MonthPayAmount = table.Column<int>(type: "int", nullable: false),
                    MonthPercent = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditRateDTO", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_CreditRateDTO_CreditRateId",
                table: "Credits",
                column: "CreditRateId",
                principalTable: "CreditRateDTO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
