using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class AddingProductsToTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductId", "City", "Description", "ImagePath", "Price", "ProductTypeId", "Quantity", "Title", "UserId" },
                values: new object[] { 13, null, "Really blends it well", null, 19.989999999999998, 2, 2, "Blender", "3abf5f13-de66-480c-adad-8d1e1eecf318" });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductId", "City", "Description", "ImagePath", "Price", "ProductTypeId", "Quantity", "Title", "UserId" },
                values: new object[] { 14, null, "Sharp", null, 59.990000000000002, 2, 1, "Knife Set", "3abf5f13-de66-480c-adad-8d1e1eecf318" });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductId", "City", "Description", "ImagePath", "Price", "ProductTypeId", "Quantity", "Title", "UserId" },
                values: new object[] { 15, null, "Black and Sleek", null, 100.0, 2, 2, "Microwave", "3abf5f13-de66-480c-adad-8d1e1eecf318" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 5);

        }
    }
}
