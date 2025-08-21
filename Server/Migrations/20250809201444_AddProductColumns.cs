using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReichertsMeatDistributing.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddProductColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the Products table exists, if not create it
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""Products"" (
                    ""ItemID"" TEXT NOT NULL CONSTRAINT ""PK_Products"" PRIMARY KEY,
                    ""StockingUM"" TEXT NOT NULL,
                    ""ItemDescription"" TEXT NOT NULL
                );
            ");

            // Add new columns to existing Products table
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "datetime('now')");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Products",
                type: "TEXT",
                nullable: true);

            // Create WeeklyDeals table if it doesn't exist
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""WeeklyDeals"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_WeeklyDeals"" PRIMARY KEY AUTOINCREMENT,
                    ""Name"" TEXT NOT NULL,
                    ""Description"" TEXT NOT NULL,
                    ""Price"" decimal(18,2) NOT NULL
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "WeeklyDeals");
        }
    }
}
