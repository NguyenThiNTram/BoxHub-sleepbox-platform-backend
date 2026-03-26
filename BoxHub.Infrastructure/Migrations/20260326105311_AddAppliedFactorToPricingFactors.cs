using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppliedFactorToPricingFactors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "base_factor",
                table: "pricing_factors",
                type: "numeric(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "base_factor",
                table: "pricing_factors");
        }
    }
}
