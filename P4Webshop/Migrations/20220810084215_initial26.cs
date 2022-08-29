using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P4Webshop.Migrations
{
    public partial class initial26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Smartphone",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "OrderdProducts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Mouse",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Microphone",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Keyboard",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Smartphone");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderdProducts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Mouse");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Microphone");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Keyboard");
        }
    }
}
