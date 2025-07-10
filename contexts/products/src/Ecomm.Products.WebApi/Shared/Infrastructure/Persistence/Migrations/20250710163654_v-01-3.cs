using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class v013 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "inventories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated",
                table: "inventories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "maximum_stock_level",
                table: "inventories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "minimum_stock_level",
                table: "inventories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "reserved_quantity",
                table: "inventories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "maximum_stock_level",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "minimum_stock_level",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "reserved_quantity",
                table: "inventories");
        }
    }
}
