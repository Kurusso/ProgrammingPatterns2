using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditApplication.Migrations
{
    /// <inheritdoc />
    public partial class full : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MonthPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifyDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditRateId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayingAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullMoneyAmount = table.Column<string>(type: "varchar", nullable: false),
                    MonthPayAmount = table.Column<string>(type: "varchar", nullable: false),
                    RemainingDebt = table.Column<string>(type: "varchar", nullable: false),
                    UnpaidDebt = table.Column<string>(type: "varchar", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifyDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credits_CreditRates_CreditRateId",
                        column: x => x.CreditRateId,
                        principalTable: "CreditRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credits_CreditRateId",
                table: "Credits",
                column: "CreditRateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "CreditRates");
        }
    }
}
