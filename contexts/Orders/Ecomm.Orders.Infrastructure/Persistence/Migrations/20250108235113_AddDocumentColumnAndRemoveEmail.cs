using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomm.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentColumnAndRemoveEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Customers",
                newName: "Document");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Document",
                table: "Customers",
                newName: "Email");
        }
    }
}
