using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class NewOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "DateCompleted", "PaymentTypeId", "UserId" },
                values: new object[] { 2, null, null, "3abf5f13-de66-480c-adad-8d1e1eecf318" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2);
        }
    }
}
