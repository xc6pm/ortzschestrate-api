using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ortzschestrate.Data.Migrations
{
    /// <inheritdoc />
    public partial class WalletAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnverifiedWalletAddress",
                table: "AspNetUsers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress",
                table: "AspNetUsers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnverifiedWalletAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WalletAddress",
                table: "AspNetUsers");
        }
    }
}
