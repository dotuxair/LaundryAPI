using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP.API.Migrations
{
    /// <inheritdoc />
    public partial class RetailerAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Retailer_RetailerId",
                table: "Machines");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Retailer_RetailerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Retailer_Users_UserId",
                table: "Retailer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Retailer",
                table: "Retailer");

            migrationBuilder.RenameTable(
                name: "Retailer",
                newName: "Retailers");

            migrationBuilder.RenameIndex(
                name: "IX_Retailer_UserId",
                table: "Retailers",
                newName: "IX_Retailers_UserId");

            migrationBuilder.AddColumn<string>(
                name: "SpinSpeed",
                table: "Programs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Retailers",
                table: "Retailers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_Retailers_RetailerId",
                table: "Machines",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Retailers_RetailerId",
                table: "Products",
                column: "RetailerId",
                principalTable: "Retailers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Retailers_Users_UserId",
                table: "Retailers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Retailers_RetailerId",
                table: "Machines");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Retailers_RetailerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Retailers_Users_UserId",
                table: "Retailers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Retailers",
                table: "Retailers");

            migrationBuilder.DropColumn(
                name: "SpinSpeed",
                table: "Programs");

            migrationBuilder.RenameTable(
                name: "Retailers",
                newName: "Retailer");

            migrationBuilder.RenameIndex(
                name: "IX_Retailers_UserId",
                table: "Retailer",
                newName: "IX_Retailer_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Retailer",
                table: "Retailer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_Retailer_RetailerId",
                table: "Machines",
                column: "RetailerId",
                principalTable: "Retailer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Retailer_RetailerId",
                table: "Products",
                column: "RetailerId",
                principalTable: "Retailer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Retailer_Users_UserId",
                table: "Retailer",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
