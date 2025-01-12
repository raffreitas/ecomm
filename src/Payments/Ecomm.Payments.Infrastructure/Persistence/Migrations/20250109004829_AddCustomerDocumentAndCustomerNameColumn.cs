using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.Payments.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerDocumentAndCustomerNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerDocument",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerDocument",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Payments");
        }
    }
}
