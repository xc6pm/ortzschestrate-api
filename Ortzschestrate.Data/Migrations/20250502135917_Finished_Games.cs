using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ortzschestrate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Finished_Games : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinishedGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerColors = table.Column<byte[]>(type: "smallint[]", nullable: false),
                    StakeEth = table.Column<double>(type: "double precision", nullable: false),
                    TimeInMs = table.Column<int>(type: "integer", nullable: false),
                    Started = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemainingTimesInMs = table.Column<int[]>(type: "integer[]", nullable: false),
                    Pgn = table.Column<string>(type: "text", nullable: false),
                    EndGameType = table.Column<byte>(type: "smallint", nullable: false),
                    WonSide = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinishedGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinishedGameUser",
                columns: table => new
                {
                    FinishedGamesId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinishedGameUser", x => new { x.FinishedGamesId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_FinishedGameUser_AspNetUsers_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinishedGameUser_FinishedGames_FinishedGamesId",
                        column: x => x.FinishedGamesId,
                        principalTable: "FinishedGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinishedGameUser_PlayersId",
                table: "FinishedGameUser",
                column: "PlayersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinishedGameUser");

            migrationBuilder.DropTable(
                name: "FinishedGames");
        }
    }
}
