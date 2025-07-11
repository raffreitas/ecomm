using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.Products.WebApi.Shared.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class v200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_category_categories_category_id",
                table: "product_category");

            migrationBuilder.DropForeignKey(
                name: "fk_product_category_products_product_id",
                table: "product_category");

            migrationBuilder.DropPrimaryKey(
                name: "pk_product_category",
                table: "product_category");

            migrationBuilder.RenameTable(
                name: "product_category",
                newName: "products_categories");

            migrationBuilder.RenameIndex(
                name: "ix_product_category_category_id",
                table: "products_categories",
                newName: "ix_products_categories_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products_categories",
                table: "products_categories",
                columns: new[] { "product_id", "category_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_products_categories_categories_category_id",
                table: "products_categories",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_products_categories_products_product_id",
                table: "products_categories",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_products_categories_categories_category_id",
                table: "products_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_products_categories_products_product_id",
                table: "products_categories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_products_categories",
                table: "products_categories");

            migrationBuilder.RenameTable(
                name: "products_categories",
                newName: "product_category");

            migrationBuilder.RenameIndex(
                name: "ix_products_categories_category_id",
                table: "product_category",
                newName: "ix_product_category_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_product_category",
                table: "product_category",
                columns: new[] { "product_id", "category_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_product_category_categories_category_id",
                table: "product_category",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_product_category_products_product_id",
                table: "product_category",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
