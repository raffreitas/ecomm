using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class v012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_inventories_product_id",
                table: "inventories",
                column: "product_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventories");
        }
    }
}
