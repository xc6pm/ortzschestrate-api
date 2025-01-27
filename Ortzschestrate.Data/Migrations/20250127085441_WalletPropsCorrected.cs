using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ortzschestrate.Data.Migrations
{
    /// <inheritdoc />
    public partial class WalletPropsCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "AspNetUsers",
                type: "character varying(42)",
                maxLength: 42,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "UnverifiedWalletAddress",
                table: "AspNetUsers",
                type: "character varying(42)",
                maxLength: 42,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WalletAddress",
                table: "AspNetUsers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(42)",
                oldMaxLength: 42,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnverifiedWalletAddress",
                table: "AspNetUsers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(42)",
                oldMaxLength: 42,
                oldNullable: true);
        }
    }
}
