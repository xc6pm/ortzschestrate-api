using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ortzschestrate.Data.Migrations
{
    /// <inheritdoc />
    public partial class GameTime_As_Double : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TimeInMs",
                table: "FinishedGames",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double[]>(
                name: "RemainingTimesInMs",
                table: "FinishedGames",
                type: "double precision[]",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TimeInMs",
                table: "FinishedGames",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int[]>(
                name: "RemainingTimesInMs",
                table: "FinishedGames",
                type: "integer[]",
                nullable: false,
                oldClrType: typeof(double[]),
                oldType: "double precision[]");
        }
    }
}
