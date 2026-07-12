using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
